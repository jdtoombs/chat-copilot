using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public record CompletionDeploymentModel(string OpenAIDeploymentId, string Name, string SpecializationId)
    : IStorageEntity
{
    public required string Id { get; set; }
    public string Partition => this.SpecializationId;
}
