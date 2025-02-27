using System.ComponentModel;
using CopilotChat.WebApi.Services;
using Microsoft.SemanticKernel;

namespace CopilotChat.WebApi.Plugins;

public class EmailPlugin(IEmailSender emailSender)
{
    [KernelFunction, Description("Send an email")]
    public void SendEmail(
        [Description("Email of the sender")] string sender,
        [Description("Email of the recipient")] string recipient,
        [Description("Email subject line")] string subject,
        [Description("Email message body")] string body
    ) => emailSender.Send(sender, recipient, subject, body);
}
