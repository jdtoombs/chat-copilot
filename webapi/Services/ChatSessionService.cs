using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

public class ChatSessionService(
    ChatSessionRepository chatSessionRepository,
    ISpecializationService specializationService,
    IOpenAIDeploymentService openAIDeploymentService,
    ICompletionDeploymentModelService completionDeploymentModelService
) : IChatSessionService
{
    public async Task<OpenAIDeployment> GetDeployment(string chatId)
    {
        var chatSession = await chatSessionRepository.FindByIdAsync(chatId);
        var specialization = await specializationService.GetSpecializationAsync(chatSession.specializationId);
        var completionDeploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specialization.Id
        );
        return await openAIDeploymentService.GetDeployment(completionDeploymentModel.OpenAIDeploymentId);
    }

    public async Task<ChatCompletionDeployment?> GetCompletionDeployment(string chatId)
    {
        var chatSession = await chatSessionRepository.FindByIdAsync(chatId);
        var specialization = await specializationService.GetSpecializationAsync(chatSession.specializationId);
        var completionDeploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specialization.Id
        );
        var deployment = await openAIDeploymentService.GetDeployment(completionDeploymentModel.OpenAIDeploymentId);

        return deployment
            .ChatCompletionDeployments.Where(chatCompletionDeployment =>
                chatCompletionDeployment.Name == completionDeploymentModel.Name
            )
            .FirstOrDefault();
    }

    public async Task<string?> GetImageGenerationDeployment(string chatId)
    {
        var chatSession = await chatSessionRepository.FindByIdAsync(chatId);
        var specialization = await specializationService.GetSpecializationAsync(chatSession.specializationId);
        var completionDeploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specialization.Id
        );
        var deployment = await openAIDeploymentService.GetDeployment(completionDeploymentModel.OpenAIDeploymentId);

        return deployment
            .ImageGenerationDeployments
            // TODO: store this in a specialization
            .Where(imageGenerationDeployment => imageGenerationDeployment == "dall-e-3")
            .FirstOrDefault();
    }
}
