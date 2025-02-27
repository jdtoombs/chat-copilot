// Copyright (c) Quartech. All rights reserved.

using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

/// <summary>
/// A repository for chat users.
/// </summary>
internal class ChatUserRepository(IStorageContext<ChatUser> storageContext) : Repository<ChatUser>(storageContext) { }
