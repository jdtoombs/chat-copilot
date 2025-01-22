// Copyright (c) Quartech. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
public class SessionlessChatController(ISingleMessageCompletionService singleMessageCompletionService) : ControllerBase
{
    [Route("sessionless/chat")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> SessionlessChatAsync([FromBody] Ask ask, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ask.Input))
        {
            return this.StatusCode(400, "No text input provided!");
        }

        var textResponse = await singleMessageCompletionService.GetResponse(ask.Input, cancellationToken);

        return this.Ok(new { value = textResponse, variables = Array.Empty<object>() });
    }
}
