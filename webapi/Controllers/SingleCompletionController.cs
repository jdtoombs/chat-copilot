// Copyright (c) Quartech. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CompletionsController(
    ISingleMessageCompletionService singleMessageCompletionService,
    IQSpecializationService qSpecializationService
) : ControllerBase
{
    [Route("chats")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> SessionlessChatAsync(
        [FromBody] Ask ask,
        [FromQuery] string? specializationId,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(ask.Input))
        {
            return this.StatusCode(400, "No text input provided!");
        }
        Specialization? spec = null;
        if (!string.IsNullOrEmpty(specializationId))
        {
            spec = await qSpecializationService.GetSpecializationAsync(specializationId);
        }

        var textResponse = await singleMessageCompletionService.GetResponse(ask.Input, spec, cancellationToken);

        return this.Ok(new AskResult { Value = textResponse });
    }
}
