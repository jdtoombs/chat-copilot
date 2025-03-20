using System.Text.Json.Serialization;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Models.Response;

public class SpecializationIndexResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("queryType")]
    public string QueryType { get; set; } = string.Empty;

    [JsonPropertyName("aiSearchDeploymentId")]
    public string AISearchDeploymentId { get; set; } = string.Empty;

    [JsonPropertyName("openAIDeploymentConnection")]
    public string OpenAIDeploymentConnection { get; set; } = string.Empty;

    [JsonPropertyName("embeddingDeployment")]
    public string EmbeddingDeployment { get; set; } = string.Empty;

    public SpecializationIndexResponse(SpecializationIndex index)
    {
        this.Id = index.Id;
        this.Label = index.Label;
        this.QueryType = index.QueryType;
        this.Name = index.Name;
        this.AISearchDeploymentId = index.AISearchDeploymentId;
        this.OpenAIDeploymentConnection = index.OpenAIDeploymentConnection;
        this.EmbeddingDeployment = index.EmbeddingDeployment;
    }
}
