// Copyright (c) Microsoft. All rights reserved.

namespace CopilotChat.WebApi.Auth;

internal interface IAuthInfo
{
    /// <summary>
    /// The authenticated user's unique ID.
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// The authenticated user's name.
    /// </summary>
    string Name { get; }
}
