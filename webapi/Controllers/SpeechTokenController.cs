// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CopilotChat.WebApi.Controllers;

[ApiController]
internal class SpeechTokenController(
    IOptions<AzureSpeechOptions> options,
    ILogger<SpeechTokenController> logger,
    IHttpClientFactory httpClientFactory
) : ControllerBase
{
    private sealed class TokenResult
    {
        public string? Token { get; set; }
        public HttpStatusCode? ResponseCode { get; set; }
    }

    /// <summary>
    /// Get an authorization token and region
    /// </summary>
    [Route("speechToken")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<SpeechTokenResponse>> GetAsync()
    {
        // Azure Speech token support is optional. If the configuration is missing or incomplete, return an unsuccessful token response.
        if (string.IsNullOrWhiteSpace(options.Value.Region) || string.IsNullOrWhiteSpace(options.Value.Key))
        {
            return new SpeechTokenResponse { IsSuccess = false };
        }

        string fetchTokenUri = "https://" + options.Value.Region + ".api.cognitive.microsoft.com/sts/v1.0/issueToken";

        TokenResult tokenResult = await this.FetchTokenAsync(fetchTokenUri, options.Value.Key);
        var isSuccess = tokenResult.ResponseCode != HttpStatusCode.NotFound;
        return new SpeechTokenResponse
        {
            Token = tokenResult.Token,
            Region = options.Value.Region,
            IsSuccess = isSuccess,
        };
    }

    private async Task<TokenResult> FetchTokenAsync(string fetchUri, string subscriptionKey)
    {
        using var client = httpClientFactory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Post, fetchUri);
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

        var result = await client.SendAsync(request);
        if (result.IsSuccessStatusCode)
        {
            var response = result.EnsureSuccessStatusCode();
            logger.LogDebug("Token Uri: {0}", fetchUri);
            string token = await result.Content.ReadAsStringAsync();
            return new TokenResult { Token = token, ResponseCode = response.StatusCode };
        }

        return new TokenResult { ResponseCode = HttpStatusCode.NotFound };
    }
}
