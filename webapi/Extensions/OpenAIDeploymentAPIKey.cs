using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Extensions;

public class OpenAIDeploymentAPIKey
{
    public OpenAIDeploymentAPIKey(OpenAIDeployment deployment, string apiKey)
    {
        this.Deployment = deployment;
        this.ApiKey = apiKey;
    }

    public OpenAIDeployment Deployment { get; set; }
    public string ApiKey { get; set; }
}
