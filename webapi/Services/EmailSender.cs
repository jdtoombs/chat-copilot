using System.Net.Mail;

namespace CopilotChat.WebApi.Services;

internal class EmailSender(SmtpClient client) : IEmailSender
{
    public void Send(string sender, string recipient, string subject, string body)
    {
        using var message = new MailMessage(sender, recipient, subject, body);
        message.ReplyToList.Add(new MailAddress(recipient));

        client.Send(message);
    }
}
