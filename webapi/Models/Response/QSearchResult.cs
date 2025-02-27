// Copyright (c) Quartech. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Response;

/// <summary>
/// Response definition for search response
/// This model is built with AzureAISearch response structure as base model.
/// </summary>
internal class QSearchResult
{
    [JsonPropertyName("count")]
    public int count { get; set; }

    [JsonPropertyName("value")]
    public IEnumerable<QSearchResultValue>? values { get; set; } = new List<QSearchResultValue>();
}

internal class QSearchResultValue
{
    [JsonPropertyName("matches")]
    public IEnumerable<QSearchMatch>? matches { get; set; } = new List<QSearchMatch>();

#pragma warning restore CA1056 // URI-like properties should not be strings

    [JsonPropertyName("filename")]
    public string? filename { get; set; }
}

internal class QSearchMatch
{
    public string id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string label { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<string> content { get; set; } = new List<string>();

    [JsonPropertyName("metadata")]
    public QSearchMetadata metadata { get; set; } = new QSearchMetadata();
}

internal class QSearchMetadata
{
    [JsonPropertyName("page_number")]
    public int pageCount { get; set; } = 0;

    [JsonPropertyName("source")]
    public QSearchMetadataSource source { get; set; } = new QSearchMetadataSource();
}

internal class QSearchMetadataSource
{
    [JsonPropertyName("filename")]
    public string filename { get; set; } = string.Empty;

    [JsonPropertyName("url")]
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string url { get; set; } = string.Empty;
#pragma warning restore CA1056 // URI-like properties should not be strings
}
