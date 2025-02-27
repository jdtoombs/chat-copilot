using CopilotChat.Shared;

namespace CopilotChat.WebApi.Extensions;

internal class DefaultConfigurationFactory(IDefaultConfigurationAccessor accessor) : IDefaultConfigurationFactory
{
    public DefaultConfiguration GetDefaultConfiguration() =>
        accessor.CreateDefaultConfigurationAsync().GetAwaiter().GetResult();
}
