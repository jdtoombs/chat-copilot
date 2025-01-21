// Copyright (c) Quartech. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService(Kernel kernel)
{
    /// <summary>
    /// Retrieves the chat completion using semantic kernel.
    /// no specialization
    /// </summary>
    public async Task<string> GetResponse(string userPrompt, CancellationToken cancellationToken)
    {
        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var stream = await chatCompletion.GetChatMessageContentAsync(
            userPrompt,
            null, //specialization
            kernel,
            cancellationToken
        );
        return stream?.Content ?? "no response!";
    }
}
