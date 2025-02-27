using Microsoft.AspNetCore.Http;

namespace CopilotChat.WebApi.Context;

internal class ContextValueAccessor(IHttpContextAccessor contextAccessor) : IContextValueAccessor
{
    public object? GetRouteValue(string key)
    {
        var context = contextAccessor.HttpContext;

        if (context == null)
        {
            return null;
        }

        context.Request.RouteValues.TryGetValue(key, out var routeValue);

        return routeValue;
    }
}
