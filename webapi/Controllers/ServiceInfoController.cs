// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for returning information on the service.
/// </summary>
[ApiController]
internal class ServiceInfoController(
    IConfiguration configuration,
    IOptions<KernelMemoryConfig> memoryOptions,
    IOptions<ChatAuthenticationOptions> chatAuthenticationOption,
    IOptions<FrontendOptions> frontendOptions,
    IDictionary<string, Plugin> availablePlugins,
    IOptions<ContentSafetyOptions> contentSafetyOptions
) : ControllerBase
{
    /// <summary>
    /// Return information on running service.
    /// </summary>
    [Route("info")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetServiceInfo()
    {
        var response = new ServiceInfoResponse()
        {
            MemoryStore = new MemoryStoreInfoResponse()
            {
                Types = Enum.GetNames(typeof(MemoryStoreType)),
                SelectedType = memoryOptions.Value.GetMemoryStoreType(configuration).ToString(),
            },
            AvailablePlugins = this.SanitizePlugins(availablePlugins),
            Version = GetAssemblyFileVersion(),
            IsContentSafetyEnabled = contentSafetyOptions.Value.Enabled,
        };

        return this.Ok(response);
    }

    /// <summary>
    /// Return the auth config to be used by the frontend client to access this service.
    /// </summary>
    [Route("authConfig")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public IActionResult GetAuthConfig()
    {
        string authorityUriString = string.Empty;
        if (
            !string.IsNullOrEmpty(chatAuthenticationOption.Value.AzureAd!.Instance)
            && !string.IsNullOrEmpty(chatAuthenticationOption.Value.AzureAd!.TenantId)
        )
        {
            var authorityUri = new Uri(chatAuthenticationOption.Value.AzureAd!.Instance);
            authorityUri = new Uri(authorityUri, chatAuthenticationOption.Value.AzureAd!.TenantId);
            authorityUriString = authorityUri.ToString();
        }

        var config = new FrontendAuthConfig
        {
            AuthType = chatAuthenticationOption.Value.Type.ToString(),
            AadAuthority = authorityUriString,
            AadClientId = frontendOptions.Value.AadClientId,
            AadApiScope =
                $"api://{chatAuthenticationOption.Value.AzureAd!.ClientId}/{chatAuthenticationOption.Value.AzureAd!.Scopes}",
        };

        return this.Ok(config);
    }

    private static string GetAssemblyFileVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

        return fileVersion.FileVersion ?? string.Empty;
    }

    /// <summary>
    /// Sanitize the plugins to only return the name and url.
    /// </summary>
    /// <param name="plugins">The plugins to sanitize.</param>
    /// <returns></returns>
    private IEnumerable<Plugin> SanitizePlugins(IDictionary<string, Plugin> plugins)
    {
        return plugins.Select(p => new Plugin() { Name = p.Value.Name, ManifestDomain = p.Value.ManifestDomain });
    }
}
