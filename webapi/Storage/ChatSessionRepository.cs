// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

/// <summary>
/// A repository for chat sessions.
/// </summary>
internal class ChatSessionRepository(IStorageContext<ChatSession> storageContext)
    : Repository<ChatSession>(storageContext)
{
    /// <summary>
    /// Retrieves a list of chat sessions.
    /// </summary>
    /// <param name="chatIds">List of ChatSession IDs to retrieve.</param>
    /// <returns>A list of ChatSessions.</returns>
    public Task<IEnumerable<ChatSession>> FindByIdsAsync(IEnumerable<string> chatIds)
    {
        var chatSet = new HashSet<string>(chatIds);
        return base.StorageContext.QueryEntitiesAsync(e => chatSet.Contains(e.Partition));
    }

    public Task<IEnumerable<ChatSession>> QueryEntitiesAsync(Expression<Func<ChatSession, bool>> predicate) =>
        base.StorageContext.QueryEntitiesAsync(predicate);
}
