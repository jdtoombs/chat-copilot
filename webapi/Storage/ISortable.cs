using System.Linq;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Storage;

internal interface ISortable<T, TSortOption>
{
    IQueryable<T> Sort(IQueryable<T> queryable, TSortOption? sortOption);
}

internal interface ICopilotChatMessageSortable : ISortable<CopilotChatMessage, CopilotChatMessageSortOption?>
{
    new IQueryable<CopilotChatMessage> Sort(
        IQueryable<CopilotChatMessage> queryable,
        CopilotChatMessageSortOption? sortOption
    );
}
