// Copyright (c) Quartech. All rights reserved.

using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Models.Response;

/// <summary>
/// The response body for loading user settings
/// </summary>
internal class LoadSettingsResponse
{
    public ChatUserSettings? settings { get; set; }
    public string adminGroupId { get; set; } = string.Empty;
}
