// Copyright (c) Microsoft. All rights reserved.
using System;
using System.Threading.Tasks;
using CopilotChat.WebApi.Extensions;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
public class SessionlessChatController : ControllerBase
{
    private readonly SingleMessageCompletionService _singleMessageCompletionService;

    /*    public SessionlessChatController(
        SingleMessageCompletionService singleMessageCompletionService
    )
    {
        this._singleMessageCompletionService = singleMessageCompletionService;
    }
    */
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
       // [FromServices] Kernel kernel,
        [FromBody] Ask ask
    )
    {

        var Test = this._singleMessageCompletionService.GetResponse();
        // KernelFunction? chatFunction = kernel.Plugins.GetFunction(ChatPluginName, ChatFunctionName);
        return this.Ok(new { Success = true, Response = Test });
    }

}