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

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Auth;
using CopilotChat.WebApi.Hubs;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Options;
using CopilotChat.WebApi.Plugins.Chat;
using CopilotChat.WebApi.Services;
using CopilotChat.WebApi.Storage;
using CopilotChat.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.MsGraph;
using Microsoft.SemanticKernel.Plugins.MsGraph.Connectors;
using Microsoft.SemanticKernel.Plugins.MsGraph.Connectors.Client;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Microsoft.KernelMemory;

using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using OpenAI.Chat;

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
        [FromServices] Kernel kernel,
        [FromBody] Ask ask,
        CancellationToken cancellationToken
    )
    {

        var Test = this._singleMessageCompletionService.GetResponse(ask.Input, kernel, cancellationToken);
        // KernelFunction? chatFunction = kernel.Plugins.GetFunction(ChatPluginName, ChatFunctionName);
        return this.Ok(new { Success = true, Response = Test });
    }

}