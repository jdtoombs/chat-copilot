// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Auth;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Hubs;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Options;
using CopilotChat.WebApi.Plugins.Utils;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;
using CopilotChat.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller for chat history.
/// This controller is responsible for creating new chat sessions, retrieving chat sessions,
/// retrieving chat messages, and editing chat sessions.
/// Note: This class has been modified to support chat specialization.
/// </summary>
[ApiController]
public class ChatHistoryController(
    ILogger<ChatHistoryController> logger,
    IKernelMemory memoryClient,
    ChatSessionRepository sessionRepository,
    ChatMessageRepository messageRepository,
    ChatParticipantRepository participantRepository,
    ChatMemorySourceRepository sourceRepository,
    IOptions<PromptsOptions> promptOptions,
    ISpecializationService specializationService,
    IAuthInfo authInfo
) : ControllerBase
{
    private const string ChatEditedClientCall = "ChatEdited";
    private const string ChatDeletedClientCall = "ChatDeleted";
    private const string ChatHistoryDeletedClientCall = "ChatHistoryDeleted";
    private const string GetChatRoute = "GetChatRoute";

    /// <summary>
    /// Create a new chat session and populate the session with the initial bot message.
    /// </summary>
    /// <param name="chatParameter">Contains the title of the chat.</param>
    /// <returns>The HTTP action result.</returns>
    [HttpPost]
    [Route("chats")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = AuthPolicyName.RequireSpecialization)]
    public async Task<IActionResult> CreateChatSessionAsync([FromBody] CreateChatParameters chatParameters)
    {
        if (chatParameters.Title == null || chatParameters.specializationId == null)
        {
            return this.BadRequest("Chat session parameters cannot be null.");
        }

        var specialization = await specializationService.GetSpecializationAsync(chatParameters.specializationId);

        var systemDescription = promptOptions.Value.SystemDescription;
        var newChat = new ChatSession(
            chatParameters.Title,
            specialization?.RoleInformation ?? promptOptions.Value.SystemPersona,
            chatParameters.specializationId,
            chatParameters.Id
        );
        await sessionRepository.CreateAsync(newChat);
        var initialMessage = string.IsNullOrEmpty(specialization?.InitialChatMessage)
            ? promptOptions.Value.InitialBotMessage
            : specialization.InitialChatMessage;

        // Create initial bot message
        var chatMessage = CopilotChatMessage.CreateBotResponseMessage(
            newChat.Id,
            initialMessage,
            string.Empty, // The initial bot message doesn't need a prompt.
            null,
            TokenUtils.EmptyTokenUsages()
        );
        await messageRepository.CreateAsync(chatMessage);

        // Add the user to the chat session
        await participantRepository.CreateAsync(new ChatParticipant(authInfo.UserId, newChat.Id));

        var sanitizedChatId = RequestUtils.GetSanitizedParameter(newChat.Id);
        logger.LogDebug("Created chat session with id {0}.", sanitizedChatId);

        return this.CreatedAtRoute(
            GetChatRoute,
            new { chatId = newChat.Id },
            new CreateChatResponse(newChat, chatMessage)
        );
    }

    /// <summary>
    /// Get a chat session by id.
    /// </summary>
    /// <param name="chatId">The chat id.</param>
    [HttpGet]
    [Route("chats/{chatId:guid}", Name = GetChatRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<IActionResult> GetChatSessionByIdAsync(Guid chatId)
    {
        ChatSession? chat = null;
        if (await sessionRepository.TryFindByIdAsync(chatId.ToString(), callback: v => chat = v))
        {
            return this.Ok(chat);
        }

        return this.NotFound($"No chat session found for chat id '{chatId}'.");
    }

    /// <summary>
    /// Get all chat sessions associated with the logged in user. Return an empty list if no chats are found.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>A list of chat sessions. An empty list if the user is not in any chat session.</returns>
    [HttpGet]
    [Route("chats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllChatSessionsAsync()
    {
        var chatParticipants = await participantRepository.FindByUserIdAsync(authInfo.UserId);

        var chats = await sessionRepository.FindByIdsAsync(chatParticipants.Select(cp => cp.ChatId));

        return this.Ok(chats);
    }

    /// <summary>
    /// Get chat messages for a chat session.
    /// Messages are returned ordered from most recent to oldest.
    /// </summary>
    /// <param name="chatId">The chat id.</param>
    /// <param name="skip">Number of messages to skip before starting to return messages.</param>
    /// <param name="count">The number of messages to return. -1 returns all messages.</param>
    [HttpGet]
    [Route("chats/{chatId:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<IActionResult> GetChatMessagesAsync(
        [FromRoute] Guid chatId,
        [FromQuery] int skip = 0,
        [FromQuery] int count = -1
    )
    {
        var chatMessages = await messageRepository.FindByChatIdAsync(chatId.ToString(), null, skip, count);
        if (!chatMessages.Any())
        {
            return this.NotFound($"No messages found for chat id '{chatId}'.");
        }
        return this.Ok(chatMessages);
    }

    public class RateChatMessageBody
    {
        public bool? positive { get; set; }
    }

    /// <summary>
    /// Rate a chat message
    /// Returns updated message
    /// </summary>
    /// <param name="chatId">The chat id.</param>
    /// <param name="messageId">The message id.</param>
    /// <param name="body">The body containing a bool on whether feedback was positive.</param>
    [HttpPost]
    [Route("chats/{chatId:guid}/messages/{messageId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    [Authorize(Policy = AuthPolicyName.RequireSpecialization)]
    public async Task<IActionResult> RateChatMessageAsync(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        [FromBody] RateChatMessageBody body
    )
    {
        var chatMessage = await messageRepository.FindByMessageIdAsync(chatId.ToString(), messageId.ToString());
        if (chatMessage == null)
        {
            return this.NotFound($"No message found for message id '{messageId}'.");
        }
        chatMessage.UserFeedback = body.positive switch
        {
            true => Models.Storage.UserFeedback.Positive,
            false => Models.Storage.UserFeedback.Negative,
            null => null,
        };
        await messageRepository.UpsertAsync(chatMessage);

        return this.Ok(chatMessage);
    }

    /// <summary>
    /// Edit a chat session.
    /// </summary>
    /// <param name="chatParameters">Object that contains the parameters to edit the chat.</param>
    [HttpPatch]
    [Route("chats/{chatId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<IActionResult> EditChatSessionAsync(
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext,
        [FromBody] EditChatParameters chatParameters,
        [FromRoute] Guid chatId
    )
    {
        ChatSession? chat = null;
        if (await sessionRepository.TryFindByIdAsync(chatId.ToString(), callback: v => chat = v))
        {
            chat!.Title = chatParameters.Title ?? chat!.Title;
            chat!.SystemDescription = chatParameters.SystemDescription ?? chat!.SafeSystemDescription;
            chat!.MemoryBalance = chatParameters.MemoryBalance ?? chat!.MemoryBalance;
            await sessionRepository.UpsertAsync(chat);
            await messageRelayHubContext.Clients.Group(chatId.ToString()).SendAsync(ChatEditedClientCall, chat);

            return this.Ok(chat);
        }

        return this.NotFound($"No chat session found for chat id '{chatId}'.");
    }

    /// <summary>
    /// Edit a chat session to set specialization.
    /// </summary>
    /// <param name="chatParameters">Object that contains the specialization chosen for the chat.</param>
    [HttpPatch]
    [Route("chats/{chatId:guid}/specialization")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    [Authorize(Policy = AuthPolicyName.RequireSpecialization)]
    public async Task<IActionResult> EditChatSessionSpecializationAsync(
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext,
        [FromBody] EditChatSpecializationParameters chatParameters,
        [FromRoute] Guid chatId
    )
    {
        ChatSession? chat = null;
        if (await sessionRepository.TryFindByIdAsync(chatId.ToString(), callback: v => chat = v))
        {
            if (chatParameters.SpecializationId != "general")
            {
                Specialization specializationSource = await specializationService.GetSpecializationAsync(
                    chatParameters.SpecializationId
                );
                chat!.SystemDescription = specializationSource.RoleInformation;
            }
            else
            {
                chat!.SystemDescription = promptOptions.Value.SystemDescription;
            }
            chat!.specializationId = chatParameters.SpecializationId;
            await sessionRepository.UpsertAsync(chat);
            await messageRelayHubContext.Clients.Group(chatId.ToString()).SendAsync(ChatEditedClientCall, chat);

            return this.Ok(chat);
        }

        return this.NotFound($"No chat session found for chat id '{chatId}'.");
    }

    /// <summary>
    /// Gets list of imported documents for a given chat.
    /// </summary>
    [Route("chats/{chatId:guid}/documents")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<ActionResult<IEnumerable<MemorySource>>> GetSourcesAsync(Guid chatId)
    {
        logger.LogInformation("Get imported sources of chat session {0}", chatId);

        if (await sessionRepository.TryFindByIdAsync(chatId.ToString()))
        {
            IEnumerable<MemorySource> sources = await sourceRepository.FindByChatIdAsync(chatId.ToString());

            return this.Ok(sources);
        }

        return this.NotFound($"No chat session found for chat id '{chatId}'.");
    }

    /// <summary>
    /// Delete a chat session.
    /// </summary>
    /// <param name="chatId">The chat id.</param>
    [HttpDelete]
    [Route("chats/{chatId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<IActionResult> RemoveSelfFromChatAsync(
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext,
        Guid chatId,
        CancellationToken cancellationToken
    )
    {
        var chatIdString = chatId.ToString();
        ChatSession? chatToDelete = null;
        try
        {
            // Make sure the chat session exists
            chatToDelete = await sessionRepository.FindByIdAsync(chatIdString);
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound($"No chat session found for chat id '{chatId}'.");
        }

        // Delete any resources associated with the chat session.

        //await this.DeleteChatResourcesAsync(chatIdString, cancellationToken);
        var chatParticipants = await participantRepository.FindByChatIdAsync(chatIdString);
        var currentUser = chatParticipants.First((p) => p.UserId == authInfo.UserId);
        if (currentUser == null)
        {
            return this.NotFound("Current user is not in the selected chat session."); //We should never really hit this due to the auth middleware.
        }
        await participantRepository.DeleteAsync(currentUser);
        // Delete chat session and broadcast update to all participants.
        // await sessionRepository.DeleteAsync(chatToDelete);
        await messageRelayHubContext // Even though we aren't truly deleting the chat anymore, the frontend should still operate as if we were.
            .Clients.Group(chatIdString)
            .SendAsync(ChatDeletedClientCall, chatIdString, authInfo.UserId, cancellationToken: cancellationToken);

        return this.NoContent();
    }

    [HttpDelete]
    [Route("chats/all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    public async Task<IActionResult> RemoveSelfFromAllChatsAsync(
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext,
        CancellationToken cancellationToken
    )
    {
        var removedParticipants = await participantRepository.RemoveAllParticipantsForUser(authInfo.UserId);
        var chatSessionsToRemove = removedParticipants.Select((p) => p.ChatId);

        return this.Ok(chatSessionsToRemove);
    }

    /// <summary>
    /// Delete the chat history of a chat session.
    /// </summary>
    [Route("chats/{chatId:guid}/history")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = AuthPolicyName.RequireChatParticipant)]
    [Authorize(Policy = AuthPolicyName.RequireSpecialization)]
    public async Task<IActionResult> DeleteChatSessionHistoryAsync(
        [FromServices] IHubContext<MessageRelayHub> messageRelayHubContext,
        Guid chatId,
        CancellationToken cancellationToken
    )
    {
        var chatIdString = chatId.ToString();
        ChatSession? chat = null;
        try
        {
            // Make sure the chat session exists
            chat = await sessionRepository.FindByIdAsync(chatIdString);
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound($"No chat session found for chat id '{chatId}'.");
        }

        // Delete chat messages in chat session.
        try
        {
            // Create and store the tasks for deleting chat messages.
            var messages = await messageRepository.FindByChatIdAsync(chatIdString);

            var deleteTasks = new List<Task>();
            foreach (var message in messages)
            {
                deleteTasks.Add(messageRepository.DeleteAsync(message));
            }
            await Task.WhenAll(deleteTasks.ToArray());

            // Create deleted chat history bot message
            var chatMessage = CopilotChatMessage.CreateBotResponseMessage(
                chat.Id,
                promptOptions.Value.ChatHistoryDeletedMessage,
                string.Empty,
                null,
                TokenUtils.EmptyTokenUsages()
            );
            await messageRepository.CreateAsync(chatMessage);

            await messageRelayHubContext
                .Clients.Group(chatIdString)
                .SendAsync(
                    ChatHistoryDeletedClientCall,
                    chatIdString,
                    chatMessage,
                    cancellationToken: cancellationToken
                );
        }
        catch (AggregateException)
        {
            return this.StatusCode(500, $"Failed to delete chat history for chat id '{chatId}'.");
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes all associated resources (messages, memories, participants) associated with a chat session.
    /// </summary>
    /// <param name="chatId">The chat id.</param>
    /// <param name="shouldDeleteParticipants">Flag to determine if participants should be deleted.</param>
    private async Task DeleteChatResourcesAsync(string chatId, CancellationToken cancellationToken)
    {
        var cleanupTasks = new List<Task>();

        // Create and store the tasks for deleting all users tied to the chat.
        var participants = await participantRepository.FindByChatIdAsync(chatId);
        foreach (var participant in participants)
        {
            cleanupTasks.Add(participantRepository.DeleteAsync(participant));
        }

        // Create and store the tasks for deleting chat messages.
        var messages = await messageRepository.FindByChatIdAsync(chatId);
        foreach (var message in messages)
        {
            cleanupTasks.Add(messageRepository.DeleteAsync(message));
        }

        // Create and store the tasks for deleting memory sources.
        var sources = await sourceRepository.FindByChatIdAsync(chatId, false);
        foreach (var source in sources)
        {
            cleanupTasks.Add(sourceRepository.DeleteAsync(source));
        }

        // Create and store the tasks for deleting semantic memories.
        cleanupTasks.Add(
            memoryClient.RemoveChatMemoriesAsync(promptOptions.Value.MemoryIndexName, chatId, cancellationToken)
        );

        // Create a task that represents the completion of all cleanupTasks
        Task aggregationTask = Task.WhenAll(cleanupTasks);
        try
        {
            // Await the completion of all tasks in parallel
            await aggregationTask;
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occurred during the tasks
            if (
                aggregationTask?.Exception?.InnerExceptions != null
                && aggregationTask.Exception.InnerExceptions.Count != 0
            )
            {
                foreach (var innerEx in aggregationTask.Exception.InnerExceptions)
                {
                    logger.LogInformation("Failed to delete an entity of chat {0}: {1}", chatId, innerEx.Message);
                }

                throw aggregationTask.Exception;
            }

            throw new AggregateException($"Resource deletion failed for chat {chatId}.", ex);
        }
    }
}
