// Copyright (c) Quartech. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;

namespace CopilotChat.WebApi.Controllers;


/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
public class SessionlessChatController : ControllerBase
{
    private readonly SingleMessageCompletionService _singleMessageCompletionService;

    public SessionlessChatController(
        SpecializationRepository specializationSourceRepository,
        SpecializationIndexRepository specializationIndexRepository,
        OpenAIDeploymentRepository openAIDeploymentRepository,
        IOptions<QAzureOpenAIChatOptions> specializationOptions,
        ISecretClientAccessor secretClientAccessor
    )
    {
        this._singleMessageCompletionService = new SingleMessageCompletionService(
            specializationOptions.Value,
            specializationSourceRepository,
            specializationIndexRepository,
            openAIDeploymentRepository,
            secretClientAccessor.GetSecretClient()
        );
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
        [FromServices] Kernel kernel,
        [FromBody] Ask ask,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(ask.Input))
        {
            return this.StatusCode(500, "No text input provided!");
        }
        else
        {
            var textResponse = await this._singleMessageCompletionService.GetResponse(ask.Input, kernel, cancellationToken);

            return this.Ok(new { value = textResponse, variables = Array.Empty<object>() });
        }
    }
}
