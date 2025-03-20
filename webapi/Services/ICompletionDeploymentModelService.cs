using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface ICompletionDeploymentModelService
{
    Task Save(CompletionDeploymentModel completionDeploymentModel);
}
