using System.Threading;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Storage;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// Defines the contract for a service that retrieves chat completions.
/// </summary>
public interface ISingleMessageCompletionService
{
    /// <summary>
    /// Retrieves the chat completion for a given user prompt.
    /// </summary>
    /// <param name="userPrompt">The user's prompt to process.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the response string.</returns>
    Task<string> GetResponse(string userPrompt, Specialization? specialization, CancellationToken cancellationToken);
}
