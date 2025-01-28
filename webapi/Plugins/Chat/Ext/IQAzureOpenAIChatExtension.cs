using System.Threading.Tasks;
using Azure.AI.OpenAI.Chat;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Plugins.Chat.Ext;

#pragma warning disable AOAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public interface IQAzureOpenAIChatExtension
{
    string ContextKey { get; }

    Task<AzureSearchChatDataSource?> GetAzureSearchChatDataSource(Specialization? specialization);

    Task<(string? indexName, string? ApiKey, string? Endpoint)> GetAISearchDeploymentConnectionDetails(string indexId);
}
