using System.Threading.Tasks;
using CopilotChat.Shared;

namespace CopilotChat.WebApi.Extensions;

public interface IDefaultConfigurationAccessor
{
    Task<DefaultConfiguration> CreateDefaultConfigurationAsync();
}
