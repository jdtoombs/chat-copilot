// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Options;
using CopilotChat.WebApi.Plugins.Utils;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;

namespace CopilotChat.WebApi.Plugins.Chat;

/// <summary>
/// This class provides the functions to query kernel memory.
/// </summary>
public class SemanticMemoryRetriever
{
    private readonly PromptsOptions _promptOptions;

    private readonly ChatSessionRepository _chatSessionRepository;

    private readonly ISpecializationService _specializationService;

    private readonly ISpecializationIndexService _specializationIndexService;

    private readonly IKernelMemory _memoryClient;

    private readonly List<string> _memoryNames;

    /// <summary>
    /// High level logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Create a new instance of SemanticMemoryRetriever.
    /// </summary>
    public SemanticMemoryRetriever(
        IOptions<PromptsOptions> promptOptions,
        ChatSessionRepository chatSessionRepository,
        ISpecializationService specializationService,
        ISpecializationIndexService specializationIndexService,
        IKernelMemory memoryClient,
        ILogger logger
    )
    {
        this._promptOptions = promptOptions.Value;
        this._chatSessionRepository = chatSessionRepository;
        this._memoryClient = memoryClient;
        this._logger = logger;
        this._specializationIndexService = specializationIndexService;
        this._specializationService = specializationService;

        this._memoryNames = new List<string>
        {
            this._promptOptions.DocumentMemoryName,
            this._promptOptions.LongTermMemoryName,
            this._promptOptions.WorkingMemoryName,
        };
    }

    private bool TryExtractUriFromMemory(Citation.Partition memory, out Uri? uri)
    {
        uri = null;
        if (memory.Tags.TryGetValue("url", out var values))
        {
            if (values.Count != 1)
            {
                return false;
            }
            var value = values.First();
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? hyperlink))
            {
                uri = hyperlink;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Query relevant memories based on the query.
    /// </summary>
    /// <returns>A string containing the relevant memories.</returns>
    public async Task<(string, IDictionary<string, CitationSource>)> QueryMemoriesAsync(
        [Description("Query to match.")] string query,
        [Description("Chat ID to query history from")] string chatId,
        [Description("Maximum number of tokens")] int tokenLimit,
        [Description("Specialization indexes we are allowed to search on")] IEnumerable<SpecializationIndex> indexes
    )
    {
        ChatSession? chatSession = null;
        if (!await this._chatSessionRepository.TryFindByIdAsync(chatId, callback: v => chatSession = v))
        {
            throw new ArgumentException($"Chat session {chatId} not found.");
        }

        var specialization = await this._specializationService.GetSpecializationAsync(chatSession.specializationId);

        var remainingToken = tokenLimit;

        // Search for relevant memories.
        List<(Citation Citation, Citation.Partition Memory)> relevantMemories = new();
        List<Task> tasks = new();
        foreach (var memoryName in this._memoryNames)
        {
            tasks.Add(SearchMemoryAsync(this._promptOptions.MemoryIndexName, memoryName));
        }
        // Specialization index memory.
        if (specialization != null && specialization.EnableKernelMemoryMultiIndex)
        {
            foreach (var index in indexes)
            {
                tasks.Add(SearchMemoryAsync(index.Name, null));
            }
        }
        // Global document memory.
        tasks.Add(
            SearchMemoryAsync(
                this._promptOptions.MemoryIndexName,
                this._promptOptions.DocumentMemoryName,
                isGlobalMemory: true
            )
        );
        // Wait for all tasks to complete.
        await Task.WhenAll(tasks);

        var builderMemory = new StringBuilder();
        IDictionary<string, CitationSource> citationMap = new Dictionary<string, CitationSource>(
            StringComparer.OrdinalIgnoreCase
        );

        if (relevantMemories.Count > 0)
        {
            (var memoryMap, citationMap) = ProcessMemories();
            FormatMemories();
            FormatSnippets();

            /// <summary>
            /// Format long term and working memories.
            /// </summary>
            void FormatMemories()
            {
                var memoryKeys = this._promptOptions.MemoryMap.Keys.ToList();
                //indexes.ForEach(indx => memoryKeys.Add(indx.Name));

                foreach (var memoryName in memoryKeys)
                {
                    if (memoryMap.TryGetValue(memoryName, out var memories))
                    {
                        foreach ((var memoryContent, _) in memories)
                        {
                            if (builderMemory.Length == 0)
                            {
                                builderMemory.Append("Past memories (format: [memory type] <label>: <details>):\n");
                            }

                            var memoryText = $"[{memoryName}] {memoryContent}\n";
                            builderMemory.Append(memoryText);
                        }
                    }
                }
            }

            /// <summary>
            /// Format document snippets.
            /// </summary>
            void FormatSnippets()
            {
                List<(string, CitationSource)> knowledgeBaseMemories = new();
                var knowledgebaseKeys = new List<string>() { this._promptOptions.DocumentMemoryName };
                indexes.ToList().ForEach(indx => knowledgebaseKeys.Add(indx.Name));
                foreach (var key in knowledgebaseKeys)
                {
                    var gotMemory = memoryMap.TryGetValue(key, out var documentMemories);
                    if (gotMemory)
                    {
                        knowledgeBaseMemories = knowledgeBaseMemories.Concat(documentMemories).ToList();
                    }
                }
                if (knowledgeBaseMemories.Count > 0)
                {
                    builderMemory.Append(
                        "Use the following information as knowledgebase/context for your response:\n"
                            + "Quote the document link in square brackets at the end of each sentence that refers to the snippet in your response.\n"
                    );

                    foreach ((var memoryContent, var citation) in knowledgeBaseMemories)
                    {
                        var memoryText =
                            $"Document name: {citation.SourceName}\nDocument link: {citation.Link}.\n[CONTENT START]\n{memoryContent}\n[CONTENT END]\n";
                        builderMemory.Append(memoryText);
                    }
                }
            }
        }

        return (builderMemory.Length == 0 ? string.Empty : builderMemory.ToString(), citationMap);

        /// <summary>
        /// Search the memory for relevant memories by memory name.
        /// </summary>
        async Task SearchMemoryAsync(string memoryIndex, string? memoryName, bool isGlobalMemory = false)
        {
            var threshold = this.CalculateRelevanceThreshold(memoryName ?? "", chatSession!.MemoryBalance);

            var searchResult = await this._memoryClient.SearchMemoryAsync(
                memoryIndex,
                query,
                threshold,
                isGlobalMemory ? DocumentMemoryOptions.GlobalDocumentChatId.ToString() : chatId,
                memoryName
            );

            foreach (var result in searchResult.Results.SelectMany(c => c.Partitions.Select(p => (c, p))))
            {
                relevantMemories.Add(result);
            }
        }

        /// <summary>
        /// Process the relevant memories and return a map of memories with citations for each memory name.
        /// </summary>
        /// <returns>A map of memories for each memory name and a map of citations for documents.</returns>
        (IDictionary<string, List<(string, CitationSource)>>, IDictionary<string, CitationSource>) ProcessMemories()
        {
            var memoryMap = new Dictionary<string, List<(string, CitationSource)>>(StringComparer.OrdinalIgnoreCase);
            var citationMap = new Dictionary<string, CitationSource>(StringComparer.OrdinalIgnoreCase);

            foreach (var result in relevantMemories.OrderByDescending(m => m.Memory.Relevance))
            {
                var tokenCount = TokenUtils.TokenCount(result.Memory.Text);
                if (remainingToken - tokenCount > 0)
                {
                    if (result.Memory.Tags.TryGetValue(MemoryTags.TagMemory, out var tag) && tag.Count > 0)
                    {
                        var memoryName = tag.Single()!;
                        var citationSource = CitationSource.FromSemanticMemoryCitation(
                            result.Citation,
                            result.Memory.Text,
                            result.Memory.Relevance
                        );

                        if (this._memoryNames.Contains(memoryName))
                        {
                            if (!memoryMap.TryGetValue(memoryName, out var memories))
                            {
                                memories = new List<(string, CitationSource)>();
                                memoryMap.Add(memoryName, memories);
                            }

                            memories.Add((result.Memory.Text, citationSource));
                            remainingToken -= tokenCount;
                        }

                        // Only documents will have citations.
                        if (memoryName == this._promptOptions.DocumentMemoryName)
                        {
                            citationMap.TryAdd(result.Citation.Link, citationSource);
                        }
                    }
                    else if (indexes.Any(indx => indx.Name == result.Citation.Index))
                    {
                        this.TryExtractUriFromMemory(result.Memory, out Uri? url);
                        var citationSource = CitationSource.FromSemanticMemoryCitation(
                            result.Citation,
                            result.Memory.Text,
                            result.Memory.Relevance,
                            url
                        );
                        if (!memoryMap.TryGetValue(result.Citation.Index, out var memories))
                        {
                            memories = new List<(string, CitationSource)>();
                            memoryMap.Add(result.Citation.Index, memories);
                        }
                        memories.Add((result.Memory.Text, citationSource));
                        remainingToken -= tokenCount;
                        citationMap.TryAdd(result.Citation.Link, citationSource);
                    }
                }
                else
                {
                    break;
                }
            }

            return (memoryMap, citationMap);
        }
    }

    #region Private

    /// <summary>
    /// Calculates the relevance threshold for the memory.
    /// The relevance threshold is a function of the memory balance.
    /// The memory balance is a value between 0 and 1, where 0 means maximum focus on
    /// working term memory (by minimizing the relevance threshold for working memory
    /// and maximizing the relevance threshold for long term memory), and 1 means
    /// maximum focus on long term memory (by minimizing the relevance threshold for
    /// long term memory and maximizing the relevance threshold for working memory).
    /// The memory balance controls two 1st degree polynomials defined by the lower
    /// and upper bounds, one for long term memory and one for working memory.
    /// The relevance threshold is the value of the polynomial at the memory balance.
    /// </summary>
    /// <param name="memoryName">The name of the memory.</param>
    /// <param name="memoryBalance">The balance between long term memory and working term memory.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown when the memory name is invalid.</exception>
    private float CalculateRelevanceThreshold(string memoryName, float memoryBalance)
    {
        var upper = this._promptOptions.SemanticMemoryRelevanceUpper;
        var lower = this._promptOptions.SemanticMemoryRelevanceLower;

        if (memoryBalance < 0.0 || memoryBalance > 1.0)
        {
            throw new ArgumentException($"Invalid memory balance: {memoryBalance}");
        }

        if (memoryName == this._promptOptions.LongTermMemoryName)
        {
            return (lower - upper) * memoryBalance + upper;
        }
        else if (memoryName == this._promptOptions.WorkingMemoryName)
        {
            return (upper - lower) * memoryBalance + lower;
        }
        else if (memoryName == this._promptOptions.DocumentMemoryName)
        {
            return this._promptOptions.DocumentMemoryMinRelevance;
        }
        else
        {
            return this._promptOptions.SearchIndexMinRelevance; //Fall through case for when using an Azure search index, should probably handle this more specifically later
        }
    }

    # endregion
}
