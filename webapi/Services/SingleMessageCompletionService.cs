// Copyright (c) Quartech. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService(Kernel kernel, IQAzureOpenAIChatExtension qAzureOpenAIChatExtension)
    : ISingleMessageCompletionService
{
    /// <summary>
    /// Retrieves the chat completion using semantic kernel.
    /// no specialization
    /// </summary>
    public async Task<string> GetResponse(
        string userPrompt,
        Specialization? specialization,
        CancellationToken cancellationToken
    )
    {
        AzureOpenAIPromptExecutionSettings? promptSettings = null;

        if (specialization != null)
        {
            var dataSource = await qAzureOpenAIChatExtension.GetAzureSearchChatDataSource(specialization);
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            promptSettings = new AzureOpenAIPromptExecutionSettings() { AzureChatDataSource = dataSource };
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var stream = await chatCompletion.GetChatMessageContentAsync(
            userPrompt,
            promptSettings, //specialization
            kernel,
            cancellationToken
        );
        return stream?.Content ?? "no response!";
    }
}
