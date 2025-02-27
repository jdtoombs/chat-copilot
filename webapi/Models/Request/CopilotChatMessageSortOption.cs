using System.Text.Json.Serialization;

namespace CopilotChat.WebApi.Models.Request;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum CopilotChatMessageSortOption
{
    DateDesc,
    DateAsc,
    FeedbackPos,
    FeedbackNeg,
}
