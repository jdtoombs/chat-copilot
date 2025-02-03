using System.Net.Mail;

namespace CopilotChat.WebApi.Services;

public class EmailSender : IEmailSender
{
    private SmtpClient Client;

    public EmailSender()
    {
        this.Client = new SmtpClient();
    }

    public void Send(string sender, string recipient, string subject, string body)
    {
        using var message = new MailMessage(sender, recipient, subject, body);
        message.ReplyToList.Add(new MailAddress(recipient));

        this.Client.Send(message);
    }
}
