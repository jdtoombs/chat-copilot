using Azure.Security.KeyVault.Secrets;

namespace CopilotChat.WebApi.Extensions;

internal interface ISecretClientAccessor
{
    SecretClient GetSecretClient();
}
