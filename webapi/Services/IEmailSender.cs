using System.Collections.Generic;
using System.Net.Mail;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// Send email messages via simple mail transfer protocol.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email to the specified address.
    /// </summary>
    /// <param name="sender">Originating email address</param>
    /// <param name="recipient">Destination email address</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="body">Email content</param>
    void Send(
        string sender,
        string recipient,
        string subject,
        string body,
        IEnumerable<Attachment>? attachments = null
    );
}
