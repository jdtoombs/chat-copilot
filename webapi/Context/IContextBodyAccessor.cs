using System.Threading.Tasks;

namespace CopilotChat.WebApi.Context;

internal interface IContextBodyAccessor
{
    Task<T?> ReadBody<T>()
        where T : class;
}
