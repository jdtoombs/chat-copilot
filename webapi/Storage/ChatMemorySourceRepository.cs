// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

/// <summary>
/// A repository for chat messages.
/// </summary>
public class ChatMemorySourceRepository(IStorageContext<MemorySource> storageContext)
    : Repository<MemorySource>(storageContext)
{
    /// <summary>
    /// Finds chat memory sources by chat session id
    /// </summary>
    /// <param name="chatId">The chat session id.</param>
    /// <param name="includeGlobal">Flag specifying if global documents should be included in the response.</param>
    /// <returns>A list of memory sources.</returns>
    public Task<IEnumerable<MemorySource>> FindByChatIdAsync(string chatId, bool includeGlobal = true) =>
        base.StorageContext.QueryEntitiesAsync(e =>
            e.ChatId == chatId || (includeGlobal && e.ChatId == Guid.Empty.ToString())
        );

    /// <summary>
    /// Finds chat memory sources by name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>A list of memory sources with the given name.</returns>
    public Task<IEnumerable<MemorySource>> FindByNameAsync(string name) =>
        base.StorageContext.QueryEntitiesAsync(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
