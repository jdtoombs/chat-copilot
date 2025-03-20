// Copyright (c) Quartech. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Response;

/// <summary>
/// Response definition for AzureAIsearch response
/// This model is built with AzureAISearch response structure as base model.
/// </summary>
public class AzureSearchResponse
{
    [JsonPropertyName("@odata.count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public List<SearchValue> values { get; set; } = new List<SearchValue>();
}

public class SearchValue
{
    [JsonPropertyName("@search.highlights")]
    public SearchHighlight highlights { get; set; } = new SearchHighlight();

    [JsonPropertyName("url")]
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string url { get; set; } = string.Empty;
#pragma warning restore CA1056 // URI-like properties should not be strings

    [JsonPropertyName("filepath")]
    public string filename { get; set; } = string.Empty;

    public string id { get; set; } = string.Empty;
}

public class SearchHighlight
{
    [JsonPropertyName("content")]
    public List<string> content { get; set; } = new List<string>();
}
