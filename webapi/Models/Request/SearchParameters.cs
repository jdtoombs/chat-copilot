// Copyright (c) Quartech. All rights reserved.

using System.Text.Json.Serialization;

/// <summary>
/// Request definition for search
/// </summary>
namespace CopilotChat.WebApi.Models.Request;

/// <summary>
/// Request definition for search
/// This model is built by bearing the MVP requirement of supporting simple text based search.
/// </summary>
public class SearchParameters
{
    [JsonPropertyName("search")]
    public string Search { get; set; } = string.Empty;

    [JsonPropertyName("indexId")]
    public string IndexId { get; set; } = string.Empty;
}
