using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

public class OpenAIDeploymentRepository(IStorageContext<OpenAIDeployment> storageContext)
    : Repository<OpenAIDeployment>(storageContext)
{
    public Task<IEnumerable<OpenAIDeployment>> GetAllDeploymentsAsync() =>
        base.StorageContext.QueryEntitiesAsync(e => true);
}
