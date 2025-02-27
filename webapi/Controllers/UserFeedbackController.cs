using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CopilotChat.WebApi.Controllers;

[ApiController]
internal class UserFeedbackController(IUserFeedbackService userFeedbackService) : ControllerBase
{
    [HttpGet("userfeedback/search")]
    public async Task<IActionResult> SearchUserFeedback([FromQuery] UserFeedbackFilter filter)
    {
        var chatMessages = await userFeedbackService.Search(filter);

        return this.Ok(chatMessages);
    }
}
