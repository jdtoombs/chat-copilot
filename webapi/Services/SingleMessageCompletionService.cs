using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
using System.Threading;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;

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
    ){
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
    /// Retrieves the search results using AzureAISearch service.
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
            null, // null because we currently do not use specialization data to generate suggestions.
            kernel,
            cancellationToken
        );

        // Dummy implementation to avoid failure
        //await Task.Delay(500); // Simulate an async operation, e.g., API call, database access, etc.
        return stream?.Content ?? "no response!";
    }




}

