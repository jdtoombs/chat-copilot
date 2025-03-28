using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public record CompletionDeploymentModel : IStorageEntity
{
    public required string Id { get; set; }

    public string Partition => this.SpecializationId;

    public required string SpecializationId { get; set; }

    public required string OpenAIDeploymentId { get; set; }

    public required string Name { get; set; }
}
