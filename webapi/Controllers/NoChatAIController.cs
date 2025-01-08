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

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for handling chat messages and responses.
/// </summary>
[ApiController]
public class SessionlessChatController : ControllerBase, IDisposable
{
    private readonly ILogger<SessionlessChatController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly List<IDisposable> _disposables;
    private readonly ITelemetryService _telemetryService;
    private readonly ServiceOptions _serviceOptions;
    private readonly MsGraphOboPluginOptions _msGraphOboPluginOptions;
    private readonly PromptsOptions _promptsOptions;
    private readonly IDictionary<string, Plugin> _plugins;

    private const string ChatPluginName = nameof(ChatPlugin);
    private const string ChatFunctionName = "Chat";
    private const string SilentChatFunctionName = "ChatSilent";
    private const string GeneratingResponseClientCall = "ReceiveBotResponseStatus";

    public SessionlessChatController(
        ILogger<SessionlessChatController> logger,
        IHttpClientFactory httpClientFactory,
        ITelemetryService telemetryService,
        IOptions<ServiceOptions> serviceOptions,
        IOptions<MsGraphOboPluginOptions> msGraphOboPluginOptions,
        IOptions<PromptsOptions> promptsOptions,
        IDictionary<string, Plugin> plugins
    )
    {
        this._logger = logger;
        this._httpClientFactory = httpClientFactory;
        this._telemetryService = telemetryService;
        this._disposables = new List<IDisposable>();
        this._serviceOptions = serviceOptions.Value;
        this._msGraphOboPluginOptions = msGraphOboPluginOptions.Value;
        this._promptsOptions = promptsOptions.Value;
        this._plugins = plugins;
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
    CancellationToken requestAbortedToken
)
{

    //to be continued. need auth validation code and connections to AI services here
    //to return with AI response text

    return this.Ok(new { Success = true, Response = "WE GOOD!" });
}



    /// <summary>
    /// Dispose of the object.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (IDisposable disposable in this._disposables)
            {
                disposable.Dispose();
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}