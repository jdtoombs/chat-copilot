using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Request;

public class QAISearchDeploymentBase
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }

    [JsonPropertyName("secretName")]
    public string? SecretName { get; set; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}
