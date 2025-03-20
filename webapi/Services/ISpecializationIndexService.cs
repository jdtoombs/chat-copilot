using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

public interface ISpecializationIndexService
{
    Task<IEnumerable<SpecializationIndex>> GetAllIndexes();

    Task<SpecializationIndex> GetIndexAsync(string id);

    Task<SpecializationIndex> SaveIndex(SpecializationIndexCreate index);

    Task<SpecializationIndex?> UpdateIndex(Guid indexId, SpecializationIndexBase indexMutate);

    Task<SpecializationIndex?> DeleteIndex(Guid indexId);

    Task OrderSpecializations(OrderMapGuidToInt specializationOrder);
}
