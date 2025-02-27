using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Services;

public class QAISearchDeploymentService(
    AISearchDeploymentRepository searchRepository,
    ISecretClientAccessor secretClientAccessor
) : IQSearchDeploymentService
{
    public Task<IEnumerable<AISearchDeployment>> GetAllSearchDeployments()
    {
        return searchRepository.GetAllSearchDeployments();
    }

    public Task<AISearchDeployment> GetSearchDeploymentAsync(string id)
    {
        return searchRepository.FindByIdAsync(id);
    }

    public async Task<AISearchDeployment> SaveSearchDeployment(QAISearchDeploymentCreate index)
    {
        var indexInsert = new AISearchDeployment(
            index.Name,
            index.Label,
            index.Endpoint,
            index.SecretName,
            index.Order
        );
        await searchRepository.CreateAsync(indexInsert);

        return indexInsert;
    }

    public async Task<string> GetAPIKeyFromVaultForDeployment(AISearchDeployment searchDeployment)
    {
        var secretName = searchDeployment.SecretName;
        var secretValue = await secretClientAccessor.GetSecretClient().GetSecretAsync(secretName);
        return secretValue.Value.Value ?? "";
    }

    public async Task<AISearchDeployment?> UpdateSearchDeployment(Guid searchId, QAISearchDeploymentBase qSearchMutate)
    {
        var searchToEdit = await searchRepository.FindByIdAsync(searchId.ToString());
        if (searchToEdit == null)
        {
            return null;
        }

        searchToEdit.Name = qSearchMutate.Name ?? searchToEdit.Name;
        searchToEdit.Label = qSearchMutate.Label ?? searchToEdit.Label;
        searchToEdit.Endpoint = qSearchMutate.Endpoint ?? searchToEdit.Endpoint;
        searchToEdit.SecretName = qSearchMutate.SecretName ?? searchToEdit.SecretName;
        searchToEdit.Order = qSearchMutate.Order ?? searchToEdit.Order;

        await searchRepository.UpsertAsync(searchToEdit);
        return searchToEdit;
    }

    public async Task<AISearchDeployment?> DeleteSearchDeployment(Guid searchId)
    {
        var searchToDelete = await searchRepository.FindByIdAsync(searchId.ToString());
        if (searchToDelete == null)
        {
            return null;
        }
        await searchRepository.DeleteAsync(searchToDelete);
        return searchToDelete;
    }

    public async Task OrderSearchDeployments(OrderMapGuidToInt searchDeploymentOrder)
    {
        if (searchDeploymentOrder == null)
        {
            throw new ArgumentNullException(nameof(searchDeploymentOrder), "OrderMapGuidToInt must be provided.");
        }

        var indexes = (await this.GetAllSearchDeployments()).ToList();

        var upsertTasks = new List<Task>();

        foreach (var order in searchDeploymentOrder.Ordering)
        {
            string indexId = order.Key;
            int newOrder = order.Value;

            var specialization = indexes.FirstOrDefault(s => s.Id == indexId);
            if (specialization != null)
            {
                // Update the order
                specialization.Order = newOrder;
                upsertTasks.Add(searchRepository.UpsertAsync(specialization));
            }
        }
        await Task.WhenAll(upsertTasks);
    }
}
