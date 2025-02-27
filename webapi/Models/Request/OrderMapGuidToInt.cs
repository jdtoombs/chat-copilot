using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Request;

internal class OrderMapGuidToInt
{
    [JsonPropertyName("ordering")]
    public Dictionary<string, int> Ordering { get; set; } = new Dictionary<string, int>();
}
