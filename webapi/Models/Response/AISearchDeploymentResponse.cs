using System.Text.Json.Serialization;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Models.Response;

public class AISearchDeploymentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("secretName")]
    public string SecretName { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    public AISearchDeploymentResponse(AISearchDeployment search)
    {
        this.Id = search.Id;
        this.Label = search.Label;
        this.Name = search.Name;
        this.Endpoint = search.Endpoint;
        this.SecretName = search.SecretName;
    }
}
