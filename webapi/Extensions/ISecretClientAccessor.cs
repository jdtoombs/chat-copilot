using Azure.Security.KeyVault.Secrets;

namespace CopilotChat.WebApi.Extensions;

public interface ISecretClientAccessor
{
    SecretClient GetSecretClient();
}
