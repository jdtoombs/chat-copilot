using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

public class AISearchDeploymentRepository(IStorageContext<AISearchDeployment> storageContext)
    : Repository<AISearchDeployment>(storageContext)
{
    public Task<IEnumerable<AISearchDeployment>> GetAllSearchDeployments() =>
        base.StorageContext.QueryEntitiesAsync(e => true);
}
