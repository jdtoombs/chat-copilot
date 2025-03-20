using System.Threading.Tasks;
using CopilotChat.WebApi.Storage;
using CopilotChat.WebApi.Models.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CopilotChat.WebApi.Services.Implementations;

public class CompletionDeploymentModelService(CompletionDeploymentModelRepository completionDeploymentModelRepository)
    : ICompletionDeploymentModelService
{
    public Task Save(CompletionDeploymentModel completionDeploymentModel)
    {
        Validate(completionDeploymentModel);

        return completionDeploymentModelRepository.CreateAsync(completionDeploymentModel);
    }

    private void Validate(CompletionDeploymentModel completionDeploymentModel)
    {
        if (!string.IsNullOrWhiteSpace(completionDeploymentModel.IndexId))
        {
            Assert.AreNotEqual(completionDeploymentModel.Strictness, null);
            Assert.AreNotEqual(completionDeploymentModel.DocumentCount, null);
            Assert.AreNotEqual(completionDeploymentModel.MaxResponseTokenLimit, null);
            Assert.AreNotEqual(completionDeploymentModel.PastMessagesIncludedCount, null);
        }
    }
}
