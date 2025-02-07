using System;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.Shared;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
using Microsoft.Extensions.Options;

namespace CopilotChat.WebApi.Extensions;

public class DefaultConfigurationAccessor(
    IOptions<QAzureOpenAIChatOptions> qAzureOpenAIChatOptions,
    ISecretClientAccessor secretClient,
    OpenAIDeploymentRepository deploymentRepository
) : IDefaultConfigurationAccessor
{
    public async Task<DefaultConfiguration> CreateDefaultConfigurationAsync()
    {
        var options = qAzureOpenAIChatOptions.Value;

        if (!options.Enabled)
        {
            throw new InvalidOperationException("Azure OpenAI Chat is not enabled.");
        }
        var deployments = await deploymentRepository.GetAllDeploymentsAsync();
        var defaultConnection = deployments
            .ToList()
            .FirstOrDefault(conn => conn.Name.Equals(options.DefaultConnection, StringComparison.OrdinalIgnoreCase));

        if (defaultConnection == null)
        {
            throw new InvalidOperationException("Default connection not found. Please check the configuration.");
        }

        var apiKey = await secretClient.GetSecretClient().GetSecretAsync(defaultConnection.SecretName);
        return new DefaultConfiguration(
            options.DefaultModel,
            options.DefaultEmbeddingModel,
            apiKey.Value.Value,
            new Uri(defaultConnection.Endpoint)
        );
    }
}
