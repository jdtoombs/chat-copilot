using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Request;

public class AISearchDeploymentCreate : AISearchDeploymentBase
{
    [JsonPropertyName("name")]
    public new required string Name { get; set; }

    [JsonPropertyName("label")]
    public new required string Label { get; set; }

    [JsonPropertyName("secretName")]
    public new required string SecretName { get; set; }

    [JsonPropertyName("endpoint")]
    public new required string Endpoint { get; set; }

    [JsonPropertyName("order")]
    public new required int Order { get; set; }
}
