using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface IChatSessionService
{
    Task<OpenAIDeployment> GetDeployment(string chatId);
    Task<ChatCompletionDeployment?> GetCompletionDeployment(string chatId);
    Task<string?> GetImageGenerationDeployment(string chatId);
}
