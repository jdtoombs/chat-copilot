// Copyright (c) Quartech. All rights reserved.

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// The implementation class for search service.
/// </summary>
public class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler? _httpClientHandler;
    private readonly SpecializationRepository _specializationRepository;
    private IAzureOpenAIChatExtension _azureOpenAIChatExtension;

    public SearchService(
        SpecializationRepository specializationSourceRepository,
        IAzureOpenAIChatExtension azureOpenAIChatExtension
    )
    {
        this._azureOpenAIChatExtension = azureOpenAIChatExtension;
        this._httpClientHandler = new() { CheckCertificateRevocationList = true };
        this._httpClient = new(this._httpClientHandler);
        this._specializationRepository = specializationSourceRepository;
    }

    /// <summary>
    /// Retrieves the search results using AzureAISearch service.
    /// </summary>
    public async Task<SearchResult?> GetMatchesAsync(SearchParameters searchParameters)
    {
        AzureSearchRequest requestBody = new(searchParameters.Search);
        var indexId = searchParameters.IndexId;
        if (indexId == null)
        {
            return null;
        }
        var (indexName, apiKey, endpoint) = await this._azureOpenAIChatExtension.GetAISearchDeploymentConnectionDetails(
            indexId
        );
        if (indexName == null || apiKey == null || endpoint == null)
        {
            return null;
        }
        using var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{endpoint}indexes/{indexName}/docs/search?api-version=2020-06-30"),
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"),
        };
        httpRequestMessage.Headers.Add("api-Key", apiKey);
        var response = await this._httpClient.SendAsync(httpRequestMessage);
        var body = await response.Content.ReadAsStringAsync();
        var searchResponse = JsonSerializer.Deserialize<AzureSearchResponse>(body!);
        return this.formatSearchResponse(searchResponse);
    }

    /// <summary>
    /// Formatter to support the nested display of results i.e, FileName -> Matches
    /// </summary>
    private SearchResult formatSearchResponse(AzureSearchResponse? searchResponse)
    {
        if (searchResponse != null)
        {
            var groupedByfilename = searchResponse
                .values.Where(res => res.highlights != null)
                .GroupBy(value => value.filename)
                .Select(g => new SearchResultValue
                {
                    filename = g.Key,
                    matches = g.Select(
                            (value, index) =>
                                new SearchMatch
                                {
                                    id = value.id,
                                    label = "Match-" + (index + 1),
                                    content = value.highlights.content,
                                    metadata = new SearchMetadata
                                    {
                                        pageCount = 0,
                                        source = new SearchMetadataSource
                                        {
                                            filename = value.filename,
                                            url = value.url,
                                        },
                                    },
                                }
                        )
                        .ToArray(),
                });
            return new SearchResult { count = searchResponse.Count, values = groupedByfilename };
        }
        return new SearchResult();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._httpClient.Dispose();
            this._httpClientHandler?.Dispose();
        }
    }
}
