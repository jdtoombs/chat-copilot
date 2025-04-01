using System.Collections.Generic;

namespace CopilotChat.WebApi.Models.Response;

public record SpecializationReadModel
{
    public required string Id { get; set; }

    public required string Label { get; set; }

    public required string Name { get; set; }

    public string RoleInformation { get; set; } = string.Empty;

    public required string ImageFilePath { get; set; }

    public required string IconFilePath { get; set; }

    public required bool IsActive { get; set; }

    public required bool IsDefault { get; set; }

    public int? Order { get; set; }

    public IList<string> Suggestions { get; set; } = new List<string>();

    public IList<string> GroupMemberships { get; set; } = new List<string>();
}
