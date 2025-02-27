using CopilotChat.Shared;

namespace CopilotChat.WebApi.Extensions;

internal interface IDefaultConfigurationFactory
{
    DefaultConfiguration GetDefaultConfiguration();
}
