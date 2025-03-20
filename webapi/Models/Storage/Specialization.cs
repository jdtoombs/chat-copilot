// Copyright (c) Quartech. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Models.Storage;

public record Specialization : IStorageEntity
{
    public string Id { get; set; }

    public string Label { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string InitialChatMessage { get; set; }

    public string RoleInformation { get; set; }

    public IList<string> GroupMemberships { get; set; } = new List<string>();

    public string? IndexId { get; set; }

    public string OpenAIDeploymentId { get; set; }

    public string? CompletionDeploymentName { get; set; }

    public string ImageFilePath { get; set; }

    public string IconFilePath { get; set; }

    [JsonIgnore]
    public string Partition => this.Id;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = "";

    public string UpdatedBy { get; set; } = "";

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool? IsDefault { get; set; }

    public bool? RestrictResultScope { get; set; }

    public int? Strictness { get; set; }

    public int? DocumentCount { get; set; }

    public int? PastMessagesIncludedCount { get; set; }

    public int? MaxResponseTokenLimit { get; set; }

    public int? Order { get; set; }

    public IList<string> Suggestions { get; set; } = new List<string>();

    public bool CanGenImages { get; set; } = false;
}
