using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public class CompletionDeploymentModel : IStorageEntity
{
    public required string Id { get; set; }

    public required string Partition { get; set; }

    public required string OpenAIDeploymentId { get; set; }

    public required string Name { get; set; }

    public string? IndexId { get; set; }

    public bool? RestrictResultScope { get; set; }

    public int? Strictness { get; set; }

    public int? DocumentCount { get; set; }

    public int? PastMessagesIncludedCount { get; set; }

    public int? MaxResponseTokenLimit { get; set; }
}
