using System.Threading.Tasks;
using CopilotChat.Shared;

namespace CopilotChat.WebApi.Extensions;

internal interface IDefaultConfigurationAccessor
{
    Task<DefaultConfiguration> CreateDefaultConfigurationAsync();
}
