using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace CopilotChat.WebApi.Services;

public class EmailSender(SmtpClient client) : IEmailSender
{
    public void Send(
        string sender,
        string recipient,
        string subject,
        string body,
        IEnumerable<Attachment>? attachments = null
    )
    {
        using var message = new MailMessage(sender, recipient, subject, body);
        message.ReplyToList.Add(new MailAddress(recipient));
        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                message.Attachments.Add(attachment);
            }
        }
        client.Send(message);
    }

    public static Attachment CreateAttachmentFromString(string content, string filename)
    {
        var bin = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bin);

        var attachment = new Attachment(stream, filename, "text/plain");
        attachment.ContentStream.Position = 0;
        return attachment;
    }
}
