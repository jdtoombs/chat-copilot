using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Request;

public class SpecializationIndexCreate : SpecializationIndexBase
{
    [JsonPropertyName("name")]
    public new required string Name { get; set; }

    [JsonPropertyName("label")]
    public new required string Label { get; set; }

    [JsonPropertyName("queryType")]
    public new required string QueryType { get; set; }

    [JsonPropertyName("aiSearchDeploymentId")]
    public new required string AISearchDeploymentId { get; set; }

    [JsonPropertyName("openAIDeploymentConnection")]
    public new required string OpenAIDeploymentConnection { get; set; }

    [JsonPropertyName("embeddingDeployment")]
    public new required string EmbeddingDeployment { get; set; }
}
