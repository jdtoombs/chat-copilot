// Copyright (c) Quartech. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CopilotChat.WebApi.Models.Request;

public record SpecializationWriteModel
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("roleInformation")]
    public string? RoleInformation { get; set; }

    [JsonPropertyName("groupMemberships")]
    public IList<string>? GroupMemberships { get; set; }

    [JsonProperty("indexId")]
    public string? IndexId { get; set; }

    [JsonProperty("indexIds")]
    public IList<string>? IndexIds { get; set; }

    [JsonProperty("enableKernelMemoryMultiIndex")]
    public bool EnableKernelMemoryMultiIndex { get; set; } = false;

    [JsonPropertyName("isActive")]
    public bool isActive { get; set; } = true;

    [JsonPropertyName("canGenerateImages")]
    public bool CanGenImages { get; set; }

    [JsonPropertyName("openAIDeploymentId")]
    public string? OpenAIDeploymentId { get; set; }

    [JsonPropertyName("completionDeploymentName")]
    public string? CompletionDeploymentName { get; set; }

    [JsonPropertyName("initialChatMessage")]
    public string? InitialChatMessage { get; set; }

    [JsonPropertyName("isDefault")]
    public bool? IsDefault { get; set; }

    [JsonPropertyName("restrictResultScope")]
    public bool? RestrictResultScope { get; set; }

    [JsonPropertyName("strictness")]
    [Range(1, 5, ErrorMessage = "Strictness must be between 1 and 5.")]
    public int? Strictness { get; set; }

    [JsonPropertyName("documentCount")]
    [Range(3, 20, ErrorMessage = "Document count must be between 3 and 20.")]
    public int? DocumentCount { get; set; }

    [JsonPropertyName("pastMessagesIncludedCount")]
    [Range(1, 100, ErrorMessage = "Past messages included count must be between 1 and 100.")]
    public int? PastMessagesIncludedCount { get; set; }

    [JsonPropertyName("maxResponseTokenLimit")]
    [Range(1, 16384, ErrorMessage = "Max response token limit must be between 1 and 16384.")]
    public int? MaxResponseTokenLimit { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }

    [JsonPropertyName("suggestions")]
    public IList<string>? Suggestions { get; set; }
}
