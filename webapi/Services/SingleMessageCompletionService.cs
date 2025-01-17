// Copyright (c) Quartech. All rights reserved.

using CopilotChat.WebApi.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using Azure.Security.KeyVault.Secrets;
using System.Threading;
using System.Threading.Tasks;

namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService
{
    private readonly SpecializationRepository _specializationRepository;
    private QAzureOpenAIChatExtension _qAzureOpenAIChatExtension;

    public SingleMessageCompletionService(
        QAzureOpenAIChatOptions qAzureOpenAIChatOptions,
        SpecializationRepository specializationSourceRepository,
        SpecializationIndexRepository indexRepository,
        OpenAIDeploymentRepository openAIDeploymentRepository,
        SecretClient secretClient
    )
    {
        this._qAzureOpenAIChatExtension = new QAzureOpenAIChatExtension(
            qAzureOpenAIChatOptions,
            specializationSourceRepository,
            indexRepository,
            openAIDeploymentRepository,
            secretClient
        );
        this._specializationRepository = specializationSourceRepository;
    }

    /// <summary>
    /// Retrieves the chat completion using semantic kernel.
    /// no specialization
    /// </summary>
    public async Task<string> GetResponse(
        string userPrompt,
        Kernel kernel,
        CancellationToken cancellationToken
    )
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
