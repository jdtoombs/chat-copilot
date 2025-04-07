using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using CopilotChat.WebApi.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CopilotChat.WebApi.Plugins;

public class EmailPlugin(IEmailSender emailSender)
{
    [KernelFunction, Description("Send an email")]
    public void SendEmail(
        Kernel kernel,
        [Description("Email of the sender")] string sender,
        [Description("Email of the recipient")] string recipient,
        [Description("Email subject line")] string subject,
        [Description("Email message body")] string body
    )
    {
        var attachments = new List<Attachment>();
        var chatHistory = GetChatHistory(kernel.Data);
        if (chatHistory != null)
        {
            var conversationOnly = chatHistory.Where(ch =>
                ch.Role == AuthorRole.User || ch.Role == AuthorRole.Assistant
            );
            var chatHistoryStr = string.Concat(chatHistory.Select(c => c.Content + "\n\n"));
            var attachment = EmailSender.CreateAttachmentFromString(chatHistoryStr, "chat_history.txt");
            attachments.Add(attachment);
        }
        emailSender.Send(sender, recipient, subject, body, attachments);
    }

    private static ChatHistory? GetChatHistory(IDictionary<string, object?> data)
    {
        if (
            data.TryGetValue(nameof(ChatHistory), out object? chatHistoryObj)
            && chatHistoryObj is ChatHistory chatHistory
        )
        {
            return chatHistory;
        }

        return null;
    }

    // Not used at the moment, but could be useful if we wanted to add the history as plain text in the body instead of attaching it.
    private static string ConcatChatHistory(string inputString, IEnumerable<ChatMessageContent> chatHistory)
    {
        var chatContentString = string.Concat(chatHistory.Select(c => c.Content + "\n\n"));
        return string.Concat(inputString + "\nChat history when e-mail was sent:\n", chatContentString);
    }
}
