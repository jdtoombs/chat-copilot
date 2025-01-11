using System.Net.Mail;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// Send email messages via simple mail transfer protocol.
/// </summary>
public class EmailSender : IEmailSender
{
    private SmtpClient Client;

    public EmailSender()
    {
        this.Client = new SmtpClient();
    }

    /// <summary>
    /// Sends an email to the specified address.
    /// </summary>
    /// <param name="sender">Originating email address</param>
    /// <param name="recipient">Destination email address</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="body">Email content</param>
    public void Send(string sender, string recipient, string subject, string body)
    {
        using var message = new MailMessage(sender, recipient, subject, body);

        this.Client.Send(message);
    }
}
