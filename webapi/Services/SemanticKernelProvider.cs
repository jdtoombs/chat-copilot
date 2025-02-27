// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using CopilotChat.WebApi.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace CopilotChat.WebApi.Services;

#pragma warning disable SKEXP0010 // Experimental method AddAzureOpenAITextToImage, suppressed instability warning

/// <summary>
/// Extension methods for registering Semantic Kernel related services.
/// </summary>
internal sealed class SemanticKernelProvider(
    IServiceProvider serviceProvider,
    SecretClient secretClient,
    IChatSessionService chatSessionService,
    IContextValueAccessor contextValueAccessor
)
{
    private Kernel? _kernel;

    public async Task<Kernel> GetSemanticKernel()
    {
        if (this._kernel == null)
        {
            this._kernel = await this.InitializeSemanticKernel();
        }

        return this._kernel.Clone();
    }

    private async Task<Kernel> InitializeSemanticKernel()
    {
        var builder = Kernel.CreateBuilder();

        builder.Services.AddLogging();

        var chatId = contextValueAccessor.GetRouteValue("chatId")?.ToString();
        if (string.IsNullOrEmpty(chatId))
        {
            return builder.Build();
        }

        var memoryOptions = serviceProvider.GetRequiredService<IOptions<KernelMemoryConfig>>().Value;

        var openAIDeployment = await chatSessionService.GetDeployment(chatId);
        var completionDeployment = await chatSessionService.GetCompletionDeployment(chatId);
        var imageGenerationDeployment = await chatSessionService.GetImageGenerationDeployment(chatId);

        var apiKey = await secretClient.GetSecretAsync(openAIDeployment.SecretName);

        switch (memoryOptions.TextGeneratorType)
        {
            case string x when x.Equals("AzureOpenAI", StringComparison.OrdinalIgnoreCase):
            case string y when y.Equals("AzureOpenAIText", StringComparison.OrdinalIgnoreCase):
                if (completionDeployment != null)
                {
                    builder.AddAzureOpenAIChatCompletion(
                        completionDeployment.Name,
                        openAIDeployment.Endpoint,
                        apiKey.Value.Value
                    );
                }

                if (imageGenerationDeployment != null)
                {
                    builder.AddAzureOpenAITextToImage(
                        imageGenerationDeployment,
                        openAIDeployment.Endpoint,
                        apiKey.Value.Value
                    );
                }
                break;
            default:
                throw new ArgumentException(
                    $"Invalid {nameof(memoryOptions.TextGeneratorType)} value in 'KernelMemory' settings."
                );
        }

        return builder.Build();
    }
}
