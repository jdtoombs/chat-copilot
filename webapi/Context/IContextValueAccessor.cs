namespace CopilotChat.WebApi.Context;

public interface IContextValueAccessor
{
    object? GetRouteValue(string key);
}
