using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

public class CompletionDeploymentModelRepository(IStorageContext<CompletionDeploymentModel> storageContext)
    : Repository<CompletionDeploymentModel>(storageContext)
{
    public Task<IEnumerable<CompletionDeploymentModel>> FindByPartitionKey(string partitionKey) =>
        this.StorageContext.QueryEntitiesAsync(model => model.Partition == partitionKey);
}
