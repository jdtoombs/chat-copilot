using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public record CompletionDeploymentModel(
    string OpenAIDeploymentId,
    string Name,
    string? IndexId,
    bool? RestrictResultScope,
    int? Strictness,
    int? DocumentCount,
    int? PastMessagesIncludedCount,
    int? MaxResponseTokenLimit
) : IStorageEntity
{
    public required string Id { get; set; }
    public required string Partition { get; set; }
}
