// Copyright (c) Quartech. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI.Chat;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Services;

namespace CopilotChat.WebApi.Plugins.Chat.Ext;

/// <summary>
/// Chat extension class to support Azure search indexes for bot response.
/// </summary>
public class QAzureOpenAIChatExtension(
    QAzureOpenAIChatOptions qAzureOpenAIChatOptions,
    IQOpenAIDeploymentService qOpenAIDeploymentService,
    IQSpecializationIndexService qSpecializationIndexService
)
{
    /// <summary>
    /// Default specialization key.
    /// </summary>
    private string DefaultSpecialization { get; } = "general";

    /// <summary>
    /// Name of the key which carries the specialization
    /// </summary>
    public string ContextKey { get; } = "specialization";

    private bool isEnabled(string? specializationId)
    {
        return qAzureOpenAIChatOptions.Enabled && specializationId != this.DefaultSpecialization;
    }

#pragma warning disable AOAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public async Task<AzureSearchChatDataSource?> GetAzureSearchChatDataSource(Specialization? specialization)
    {
        if (
            specialization == null
            || string.IsNullOrEmpty(specialization.IndexId)
            || !this.isEnabled(specialization.Id)
        )
        {
            return null;
        }

        var qSpecializationIndex = await qSpecializationIndexService.GetIndexAsync(specialization.IndexId);
        if (qSpecializationIndex == null)
        {
            return null;
        }

        var aiSearchDeploymentConnection = qAzureOpenAIChatOptions.AISearchDeploymentConnections.FirstOrDefault(c =>
            c.Name == qSpecializationIndex.AISearchDeploymentConnection
        );
        if (aiSearchDeploymentConnection == null)
        {
            throw new InvalidOperationException("Configuration error: AI Search Deployment Connection is missing.");
        }

        var openAIDeploymentConnection = await qOpenAIDeploymentService.GetDeployment(
            specialization?.OpenAIDeploymentId ?? ""
        );
        var apiKey = await qOpenAIDeploymentService.GetAPIKeyFromVaultForDeployment(openAIDeploymentConnection);
        if (openAIDeploymentConnection == null || openAIDeploymentConnection.Endpoint == null || apiKey == null)
        {
            throw new InvalidOperationException("Configuration error: OpenAI Deployment Connection is missing.");
        }

        var embeddingEndpoint = this.GenerateEmbeddingEndpoint(
            new Uri(openAIDeploymentConnection.Endpoint),
            qSpecializationIndex
        );
        return new AzureSearchChatDataSource
        {
            IndexName = qSpecializationIndex.Name,
            Endpoint = aiSearchDeploymentConnection.Endpoint,
            Strictness = specialization.Strictness,
            FieldMappings = new DataSourceFieldMappings
            {
                UrlFieldName = null, //qSpecializationIndex.FieldMapping?.UrlFieldName,
                TitleFieldName = null, //qSpecializationIndex.FieldMapping?.TitleFieldName,
                FilePathFieldName = null, //qSpecializationIndex.FieldMapping?.FilepathFieldName,
            },
            SemanticConfiguration = "default", //qSpecializationIndex.SemanticConfiguration,
            QueryType = new DataSourceQueryType(qSpecializationIndex.QueryType),
            InScope = specialization.RestrictResultScope,
            TopNDocuments = specialization.DocumentCount,
            Authentication = DataSourceAuthentication.FromApiKey(aiSearchDeploymentConnection.APIKey),
            VectorizationSource = DataSourceVectorizer.FromEndpoint(
                embeddingEndpoint,
                DataSourceAuthentication.FromApiKey(apiKey)
            ),
        };
    }
#pragma warning restore AOAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    private Uri? GenerateEmbeddingEndpoint(Uri connectionEndpoint, SpecializationIndex qSpecializationIndex)
    {
        return new Uri(
            connectionEndpoint,
            $"/openai/deployments/{qSpecializationIndex.EmbeddingDeployment}/embeddings?api-version=2023-05-15"
        );
    }

    private QAzureOpenAIChatOptions.AISearchDeploymentConnection? GetAISearchDeploymentConnection(string connectionName)
    {
        return qAzureOpenAIChatOptions.AISearchDeploymentConnections.FirstOrDefault(connection =>
            connection.Name == connectionName
        );
    }

    public async Task<(string? indexName, string? ApiKey, string? Endpoint)> GetAISearchDeploymentConnectionDetails(
        string indexId
    )
    {
        var specializationIndex = await qSpecializationIndexService.GetIndexAsync(indexId);
        if (specializationIndex == null)
        {
            return (null, null, null);
        }
        var aiSearchDeploymentConnection = this.GetAISearchDeploymentConnection(
            specializationIndex.AISearchDeploymentConnection
        );
        return (
            specializationIndex.Name,
            aiSearchDeploymentConnection?.APIKey,
            aiSearchDeploymentConnection?.Endpoint?.ToString()
        );
    }
}
