namespace CopilotChat.WebApi.Services;

internal class IOpenAITextToImageOptions
{
    public string Deployment { get; } = string.Empty;
    public string Endpoint { get; } = string.Empty;
    public string ApiKey { get; } = string.Empty;
}
