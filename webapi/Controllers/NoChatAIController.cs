// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Context;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
public class SessionlessChatController : ControllerBase
{
    private readonly SingleMessageCompletionService _singleMessageCompletionService;
    public SessionlessChatController(
        SingleMessageCompletionService singleMessageCompletionService
    )
    {
        this._singleMessageCompletionService = singleMessageCompletionService;
    }



    [Route("sessionless/chat")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    //[Authorize(Policy = AuthPolicyName.RequireSpecialization)]
    public async Task<IActionResult> SessionlessChatAsync(
       // [FromServices] Kernel kernel,
        [FromBody] Ask ask,
        CancellationToken requestAbortedToken
    )
    {

        var Test = this._singleMessageCompletionService.GetNumber();
        // KernelFunction? chatFunction = kernel.Plugins.GetFunction(ChatPluginName, ChatFunctionName);
        return this.Ok(new { Success = true, Response = "WE GOOD!" });
    }

}