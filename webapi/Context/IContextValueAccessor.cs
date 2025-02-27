namespace CopilotChat.WebApi.Context;

internal interface IContextValueAccessor
{
    object? GetRouteValue(string key);
}
