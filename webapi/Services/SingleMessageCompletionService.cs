// Copyright (c) Quartech. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService
{
    private readonly SpecializationRepository _specializationRepository;
    private readonly Kernel _kernel;

    public SingleMessageCompletionService(SpecializationRepository specializationSourceRepository, Kernel kernel)
    {
        this._specializationRepository = specializationSourceRepository;
        this._kernel = kernel;
    }

    /// <summary>
    /// Retrieves the chat completion using semantic kernel.
    /// no specialization
    /// </summary>
    public async Task<string> GetResponse(string userPrompt, CancellationToken cancellationToken)
    {
        var chatCompletion = this._kernel.GetRequiredService<IChatCompletionService>();
        var stream = await chatCompletion.GetChatMessageContentAsync(
            userPrompt,
            null, //specialization
            this._kernel,
            cancellationToken
        );
        return stream?.Content ?? "no response!";
    }
}
