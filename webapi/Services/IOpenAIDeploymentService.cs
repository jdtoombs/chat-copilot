using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface IOpenAIDeploymentService
{
    Task<IEnumerable<OpenAIDeployment>> GetAllDeployments();

    Task<OpenAIDeployment> GetDeployment(string id);

    Task<OpenAIDeployment> SaveDeployment(OpenAIDeploymentCreate deployment);

    Task<OpenAIDeployment?> UpdateDeployment(Guid indexId, OpenAIDeploymentMutate deploymentMutate);

    Task<OpenAIDeployment?> DeleteDeployment(Guid indexId);

    Task OrderDeployments(OrderMapGuidToInt deploymentOrder);

    Task<string> GetAPIKeyFromVaultForDeployment(OpenAIDeployment deployment);
}
