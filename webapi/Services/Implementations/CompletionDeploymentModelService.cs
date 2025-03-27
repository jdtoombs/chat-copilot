using System;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services.Implementations;

public class CompletionDeploymentModelService(CompletionDeploymentModelRepository completionDeploymentModelRepository)
    : ICompletionDeploymentModelService
{
    public Task Save(CompletionDeploymentModel completionDeploymentModel, string specializationId)
    {
        var entity = completionDeploymentModel with
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationId = specializationId,
        };

        return completionDeploymentModelRepository.CreateAsync(entity);
    }

    public Task Update(CompletionDeploymentModel completionDeploymentModel) =>
        completionDeploymentModelRepository.UpsertAsync(completionDeploymentModel);

    public async Task<CompletionDeploymentModel?> FindBySpecializationId(string specializationId)
    {
        var completionDeploymentModels = await completionDeploymentModelRepository.FindByPartitionKey(specializationId);

        return completionDeploymentModels.FirstOrDefault();
    }
}
