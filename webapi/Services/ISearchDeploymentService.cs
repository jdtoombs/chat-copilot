using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface ISearchDeploymentService
{
    Task<IEnumerable<AISearchDeployment>> GetAllSearchDeployments();

    Task<AISearchDeployment> GetSearchDeploymentAsync(string id);

    Task<AISearchDeployment> SaveSearchDeployment(AISearchDeploymentCreate index);

    Task<AISearchDeployment?> UpdateSearchDeployment(Guid searchId, AISearchDeploymentBase searchMutate);

    Task<AISearchDeployment?> DeleteSearchDeployment(Guid searchId);

    Task OrderSearchDeployments(OrderMapGuidToInt searchDeploymentOrder);

    Task<string> GetAPIKeyFromVaultForDeployment(AISearchDeployment searchDeployment);
}
