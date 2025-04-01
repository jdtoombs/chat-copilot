using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Context;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

public class ChatSessionService(
    IContextValueAccessor contextValueAccessor,
    ChatSessionRepository chatSessionRepository,
    ISpecializationService specializationService,
    IOpenAIDeploymentService openAIDeploymentService,
    ICompletionDeploymentModelService completionDeploymentModelService
) : IChatSessionService
{
    private string? chatId { get; } = contextValueAccessor.GetRouteValue("chatId")?.ToString();

    public async Task<OpenAIDeployment> GetDeployment()
    {
        var specialization = await this.GetSpecialization();

        var completionDeploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specialization.Id
        );

        return await openAIDeploymentService.GetDeployment(completionDeploymentModel.OpenAIDeploymentId);
    }

    public async Task<ChatCompletionDeployment?> GetCompletionDeployment()
    {
        var specialization = await this.GetSpecialization();

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

    public async Task<string?> GetImageGenerationDeployment()
    {
        var specialization = await this.GetSpecialization();

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

    private async Task<Specialization> GetSpecialization()
    {
        if (this.chatId == null)
        {
            return await specializationService.GetDefaultSpecialization();
        }

        var chatSession = await chatSessionRepository.FindByIdAsync(this.chatId);
        return await specializationService.GetSpecializationAsync(chatSession.specializationId);
    }
}
