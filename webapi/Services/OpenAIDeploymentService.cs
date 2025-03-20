using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Storage;
using Newtonsoft.Json;

namespace CopilotChat.WebApi.Services;

public class OpenAIDeploymentService(
    OpenAIDeploymentRepository deploymentRepository,
    ISecretClientAccessor secretClientAccessor
) : IOpenAIDeploymentService
{
    public async Task<OpenAIDeployment?> DeleteDeployment(Guid indexId)
    {
        var deploymentToDelete = await deploymentRepository.FindByIdAsync(indexId.ToString());
        if (deploymentToDelete == null)
        {
            return null;
        }
        await deploymentRepository.DeleteAsync(deploymentToDelete);
        return deploymentToDelete;
    }

    public Task<IEnumerable<OpenAIDeployment>> GetAllDeployments()
    {
        return deploymentRepository.GetAllDeploymentsAsync();
    }

    public Task<OpenAIDeployment> GetDeployment(string id)
    {
        return deploymentRepository.FindByIdAsync(id);
    }

    public async Task<string> GetAPIKeyFromVaultForDeployment(OpenAIDeployment deployment)
    {
        var secretName = deployment.SecretName;
        var secretValue = await secretClientAccessor.GetSecretClient().GetSecretAsync(secretName);
        return secretValue.Value.Value ?? "";
    }

    public async Task<IEnumerable<ChatCompletionDeployment>> GetAllChatCompletionDeployments()
    {
        var deployments = await deploymentRepository.GetAllDeploymentsAsync();
        var chatCompletionDeployments = new List<ChatCompletionDeployment>();
        foreach (OpenAIDeployment connection in deployments)
        {
            foreach (var deployment in connection.ChatCompletionDeployments)
            {
                var deploymentWithConnection = new ChatCompletionDeployment
                {
                    Name = $"{deployment.Name} ({connection.Name})",
                    CompletionTokenLimit = deployment.CompletionTokenLimit,
                };
                chatCompletionDeployments.Add(deploymentWithConnection);
            }
        }
        return chatCompletionDeployments;
    }

    public async Task<OpenAIDeployment> SaveDeployment(OpenAIDeploymentCreate deployment)
    {
        var deserializeCompletions = JsonConvert.DeserializeObject<List<ChatCompletionDeployment>>(
            deployment.ChatCompletionDeployments
        );
        var deserializeEmbeddings = JsonConvert.DeserializeObject<List<string>>(deployment.EmbeddingDeployments);
        var deserializeImageGeneration = JsonConvert.DeserializeObject<List<string>>(
            deployment.ImageGenerationDeployments
        );

        var deploymentInsert = new OpenAIDeployment(
            deployment.Name,
            deployment.Endpoint,
            deployment.SecretName,
            deserializeCompletions ?? new List<ChatCompletionDeployment>(),
            deserializeImageGeneration ?? new List<string>(),
            deserializeEmbeddings ?? new List<string>()
        );

        await deploymentRepository.CreateAsync(deploymentInsert);
        return deploymentInsert;
    }

    public async Task<OpenAIDeployment?> UpdateDeployment(Guid indexId, OpenAIDeploymentMutate deploymentMutate)
    {
        var deserializeCompletions = JsonConvert.DeserializeObject<List<ChatCompletionDeployment>>(
            deploymentMutate.ChatCompletionDeployments
        );
        var deserializeEmbeddings = JsonConvert.DeserializeObject<List<string>>(deploymentMutate.EmbeddingDeployments);
        var deserializeImageGeneration = JsonConvert.DeserializeObject<List<string>>(
            deploymentMutate.ImageGenerationDeployments
        );
        var deploymentToEdit = await deploymentRepository.FindByIdAsync(indexId.ToString());

        deploymentToEdit.Name = deploymentMutate.Name ?? deploymentToEdit.Name;
        deploymentToEdit.SecretName = deploymentMutate.SecretName ?? deploymentToEdit.SecretName;
        deploymentToEdit.Endpoint = deploymentMutate.Endpoint ?? deploymentToEdit.Endpoint;
        deploymentToEdit.ChatCompletionDeployments =
            deserializeCompletions ?? deploymentToEdit.ChatCompletionDeployments;
        deploymentToEdit.EmbeddingDeployments = deserializeEmbeddings ?? deploymentToEdit.EmbeddingDeployments;
        deploymentToEdit.ImageGenerationDeployments =
            deserializeImageGeneration ?? deploymentToEdit.ImageGenerationDeployments;

        await deploymentRepository.UpsertAsync(deploymentToEdit);
        return deploymentToEdit;
    }

    public async Task OrderDeployments(OrderMapGuidToInt deploymentOrder)
    {
        if (deploymentOrder == null)
        {
            throw new ArgumentNullException(nameof(deploymentOrder), "SpecializationOrder must be provided.");
        }

        var indexes = (await this.GetAllDeployments()).ToList();

        var upsertTasks = new List<Task>();

        foreach (var order in deploymentOrder.Ordering)
        {
            string indexId = order.Key;
            int newOrder = order.Value;

            var deployment = indexes.FirstOrDefault(s => s.Id == indexId);
            if (deployment != null)
            {
                // Update the order
                deployment.Order = newOrder;
                upsertTasks.Add(deploymentRepository.UpsertAsync(deployment));
            }
        }
        await Task.WhenAll(upsertTasks);
    }
}
