using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface ICompletionDeploymentModelService
{
    Task Save(CompletionDeploymentModel completionDeploymentModel, string specializationId);

    Task Update(CompletionDeploymentModel completionDeploymentModel);

    Task<CompletionDeploymentModel?> FindBySpecializationId(string specializationId);
}
