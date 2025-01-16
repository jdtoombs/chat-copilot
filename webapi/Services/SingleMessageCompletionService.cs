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

using Microsoft.Extensions.DependencyInjection;




namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler? _httpClientHandler;
    private readonly SpecializationRepository _specializationRepository;
    private QAzureOpenAIChatExtension _qAzureOpenAIChatExtension;

    //create object
    public SingleMessageCompletionService(
        QAzureOpenAIChatOptions qAzureOpenAIChatOptions,
        SpecializationRepository specializationSourceRepository,
        SpecializationIndexRepository indexRepository,
        OpenAIDeploymentRepository openAIDeploymentRepository,
        SecretClient secretClient
    ){
        this._qAzureOpenAIChatExtension = new QAzureOpenAIChatExtension(
            qAzureOpenAIChatOptions,
            specializationSourceRepository,
            indexRepository,
            openAIDeploymentRepository,
            secretClient
        );
        this._httpClientHandler = new() { CheckCertificateRevocationList = true };
        this._httpClient = new(this._httpClientHandler);
        this._specializationRepository = specializationSourceRepository;
    }

    /// <summary>
    /// Retrieves the search results using AzureAISearch service.
    /// </summary>
    public async Task<string> GetResponse(
        string userPrompt,
        Kernel kernel,
        CancellationToken cancellationToken
    )
    {

        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var stream = await chatCompletion.GetChatMessageContentAsync(
            userPrompt,
            null, // null because we currently do not use specialization data to generate suggestions.
            kernel,
            cancellationToken
        );

        // Dummy implementation to avoid failure
        //await Task.Delay(500); // Simulate an async operation, e.g., API call, database access, etc.
        return stream?.Content ?? "no response!";
    }




}

