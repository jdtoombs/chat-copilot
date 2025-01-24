using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

public class QSpecializationIndexService(SpecializationIndexRepository indexRepository) : IQSpecializationIndexService
{
    public Task<IEnumerable<SpecializationIndex>> GetAllIndexes()
    {
        return indexRepository.GetAllIndexesAsync();
    }

    public Task<SpecializationIndex> GetIndexAsync(string id)
    {
        return indexRepository.FindByIdAsync(id);
    }

    public async Task<SpecializationIndex> SaveIndex(QSpecializationIndexCreate index)
    {
        var indexInsert = new SpecializationIndex(
            index.Name,
            index.Label,
            index.QueryType,
            index.AISearchDeploymentConnection,
            index.OpenAIDeploymentConnection,
            index.EmbeddingDeployment,
            index.Order ?? 0
        );
        await indexRepository.CreateAsync(indexInsert);

        return indexInsert;
    }

    public async Task<SpecializationIndex?> UpdateIndex(Guid indexId, QSpecializationIndexBase qIndexMutate)
    {
        var indexToEdit = await indexRepository.FindByIdAsync(indexId.ToString());
        if (indexToEdit == null)
        {
            return null;
        }

        indexToEdit.Name = qIndexMutate.Name ?? indexToEdit.Name;
        indexToEdit.Label = qIndexMutate.Label ?? indexToEdit.Label;
        indexToEdit.QueryType = qIndexMutate.QueryType ?? indexToEdit.QueryType;
        indexToEdit.AISearchDeploymentConnection =
            qIndexMutate.AISearchDeploymentConnection ?? indexToEdit.AISearchDeploymentConnection;
        indexToEdit.OpenAIDeploymentConnection =
            qIndexMutate.OpenAIDeploymentConnection ?? indexToEdit.OpenAIDeploymentConnection;
        indexToEdit.EmbeddingDeployment = qIndexMutate.EmbeddingDeployment ?? indexToEdit.EmbeddingDeployment;
        indexToEdit.Order = qIndexMutate.Order ?? indexToEdit.Order;

        await indexRepository.UpsertAsync(indexToEdit);
        return indexToEdit;
    }

    public async Task<SpecializationIndex?> DeleteIndex(Guid indexId)
    {
        var indexToDelete = await indexRepository.FindByIdAsync(indexId.ToString());
        if (indexToDelete == null)
        {
            return null;
        }
        await indexRepository.DeleteAsync(indexToDelete);
        return indexToDelete;
    }

    public async Task OrderSpecializations(OrderMapGuidToInt specializationOrder)
    {
        if (specializationOrder == null)
        {
            throw new ArgumentNullException(nameof(specializationOrder), "QSpecializationOrder must be provided.");
        }

        var indexes = (await this.GetAllIndexes()).ToList();

        var upsertTasks = new List<Task>();

        foreach (var order in specializationOrder.Ordering)
        {
            string indexId = order.Key;
            int newOrder = order.Value;

            var specialization = indexes.FirstOrDefault(s => s.Id == indexId);
            if (specialization != null)
            {
                // Update the order
                specialization.Order = newOrder;
                upsertTasks.Add(indexRepository.UpsertAsync(specialization));
            }
        }
        await Task.WhenAll(upsertTasks);
    }
}
