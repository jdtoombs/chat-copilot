using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

internal class SpecializationIndexRepository(IStorageContext<SpecializationIndex> storageContext)
    : Repository<SpecializationIndex>(storageContext)
{
    /// <summary>
    /// Retrieves all specializations.
    /// </summary>
    /// <returns>A list of specializations.</returns>
    public Task<IEnumerable<SpecializationIndex>> GetAllIndexesAsync() =>
        base.StorageContext.QueryEntitiesAsync(e => true);
}
