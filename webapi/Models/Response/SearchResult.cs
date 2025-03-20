// Copyright (c) Quartech. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Response;

#pragma warning disable CA1056 // URI-like properties should not be strings

/// <summary>
/// Response definition for search response
/// This model is built with AzureAISearch response structure as base model.
/// </summary>
public class SearchResult
{
    [JsonPropertyName("count")]
    public int count { get; set; }

    [JsonPropertyName("value")]
    public IEnumerable<SearchResultValue>? values { get; set; } = new List<SearchResultValue>();
}

public class SearchResultValue
{
    [JsonPropertyName("matches")]
    public IEnumerable<SearchMatch>? matches { get; set; } = new List<SearchMatch>();

    [JsonPropertyName("filename")]
    public string? filename { get; set; }
}

public class SearchMatch
{
    public string id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string label { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<string> content { get; set; } = new List<string>();

    [JsonPropertyName("metadata")]
    public SearchMetadata metadata { get; set; } = new SearchMetadata();
}

public class SearchMetadata
{
    [JsonPropertyName("page_number")]
    public int pageCount { get; set; } = 0;

    [JsonPropertyName("source")]
    public SearchMetadataSource source { get; set; } = new SearchMetadataSource();
}

public class SearchMetadataSource
{
    [JsonPropertyName("filename")]
    public string filename { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string url { get; set; } = string.Empty;
}
