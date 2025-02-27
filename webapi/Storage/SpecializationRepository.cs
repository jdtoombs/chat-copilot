// Copyright (c) Quartech. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

/// <summary>
/// A repository for specialization management.
/// </summary>
public class SpecializationRepository(IStorageContext<Specialization> storageContext)
    : Repository<Specialization>(storageContext)
{
    /// <summary>
    /// Retrieves all specializations.
    /// </summary>
    /// <returns>A list of specializations.</returns>
    public Task<IEnumerable<Specialization>> GetAllSpecializationsAsync() =>
        base.StorageContext.QueryEntitiesAsync(e => true);

    /// <summary>
    /// Retrieves specialization by key.
    /// </summary>
    /// <returns>A specialization matching the key.</returns>
    public async Task<Specialization> GetSpecializationAsync(string id) => await base.StorageContext.ReadAsync(id, id);
}
