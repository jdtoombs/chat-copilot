namespace CopilotChat.WebApi.Services;

public interface IEmailSender
{
    void Send(string sender, string recipient, string subject, string body);
}
