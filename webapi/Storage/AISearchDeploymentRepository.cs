using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

public class AISearchDeploymentRepository : Repository<AISearchDeployment>
{
    public AISearchDeploymentRepository(IStorageContext<AISearchDeployment> storageContext)
        : base(storageContext) { }

    public Task<IEnumerable<AISearchDeployment>> GetAllSearchDeployments()
    {
        return base.StorageContext.QueryEntitiesAsync(e => true);
    }
}
