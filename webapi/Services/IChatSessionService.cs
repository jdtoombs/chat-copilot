using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface IChatSessionService
{
    Task<OpenAIDeployment> GetDeployment();
    Task<ChatCompletionDeployment?> GetCompletionDeployment();
    Task<string?> GetImageGenerationDeployment();
}
