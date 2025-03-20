using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

public class ChatSessionService(
    ChatSessionRepository chatSessionRepository,
    ISpecializationService specializationService,
    IOpenAIDeploymentService openAIDeploymentService
) : IChatSessionService
{
    private Specialization? _specialization { get; set; }

    private async Task<Specialization> GetSpecialization(string chatId)
    {
        if (this._specialization == null)
        {
            var chatSession = await chatSessionRepository.FindByIdAsync(chatId);
            this._specialization = await specializationService.GetSpecializationAsync(chatSession.specializationId);
        }

        return this._specialization;
    }

    private OpenAIDeployment? _openAIDeployment { get; set; }

    private async Task<OpenAIDeployment> GetOpenAIDeployment(string chatId)
    {
        if (this._openAIDeployment == null)
        {
            var specialization = await this.GetSpecialization(chatId);
            this._openAIDeployment = await openAIDeploymentService.GetDeployment(specialization.OpenAIDeploymentId);
        }

        return this._openAIDeployment;
    }

    public Task<OpenAIDeployment> GetDeployment(string chatId) => this.GetOpenAIDeployment(chatId);

    public async Task<ChatCompletionDeployment?> GetCompletionDeployment(string chatId)
    {
        var specialization = await this.GetSpecialization(chatId);
        var deployment = await this.GetOpenAIDeployment(chatId);

        return deployment
            .ChatCompletionDeployments.Where(chatCompletionDeployment =>
                chatCompletionDeployment.Name == specialization.CompletionDeploymentName
            )
            .FirstOrDefault();
    }

    public async Task<string?> GetImageGenerationDeployment(string chatId)
    {
        var specialization = await this.GetSpecialization(chatId);
        var deployment = await this.GetOpenAIDeployment(chatId);

        return deployment
            .ImageGenerationDeployments
            // TODO: store this in a specialization
            .Where(imageGenerationDeployment => imageGenerationDeployment == "dall-e-3")
            .FirstOrDefault();
    }
}
