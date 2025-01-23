using System;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CopilotChat.WebApi.Controllers;

[ApiController]
public class AISearchDeploymentController(
    ILogger<AISearchDeploymentController> logger,
    IQSearchDeploymentService searchDeploymentService
) : ControllerBase
{
    [HttpGet]
    [Route("aiSearchDeployments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSearchDeploymentsAsync()
    {
        var searchDeployments = await searchDeploymentService.GetAllSearchDeployments();
        return this.Ok(searchDeployments);
    }

    [HttpPost]
    [Route("aiSearchDeployments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> SaveSearchDeployment([FromForm] QAISearchDeploymentCreate searchCreate)
    {
        try
        {
            var search = await searchDeploymentService.SaveSearchDeployment(searchCreate);
            var searchCreateResponse = new QAISearchDeploymentResponse(search);

            return this.Ok(searchCreateResponse);
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Search deployment creation threw an exception");

            return this.StatusCode(500, "Failed to create index.");
        }
    }

    [HttpPatch]
    [Route("aiSearchDeployments/{searchId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditSearchDeployment(
        [FromForm] QAISearchDeploymentBase qSearchMutate,
        [FromRoute] Guid searchId
    )
    {
        try
        {
            var searchToEdit = await searchDeploymentService.UpdateSearchDeployment(searchId, qSearchMutate);
            if (searchToEdit != null)
            {
                return this.Ok(searchToEdit);
            }
            return this.StatusCode(500, $"Failed to update search deployment for id '{searchId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Search deployment update threw an exception");

            return this.StatusCode(500, $"Failed to update search deployment for id '{searchId}'.");
        }
    }

    [HttpDelete]
    [Route("aiSearchDeployments/{searchId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSearchDeployment(Guid searchId)
    {
        try
        {
            var searchToDelete = await searchDeploymentService.DeleteSearchDeployment(searchId);
            if (searchToDelete != null)
            {
                return this.Ok(true);
            }
            return this.StatusCode(500, $"Failed to delete search deployment for id '{searchId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Search deployment delete threw an exception");

            return this.StatusCode(500, $"Failed to delete search deployment for id '{searchId}'.");
        }
    }

    [HttpPost]
    [Route("aiSearchDeployments/order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OrderSearchDeploymentsAsync([FromBody] OrderMapGuidToInt qAISearchOrder)
    {
        try
        {
            await searchDeploymentService.OrderSearchDeployments(qAISearchOrder);
            return this.NoContent();
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Search deployment swap order threw an exception");

            return this.StatusCode(500, $"Failed to order search deployments: {ex.Message}.");
        }
    }
}
