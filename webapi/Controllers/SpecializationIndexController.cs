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
internal class SpecializationIndexController(
    ILogger<SpecializationIndexController> logger,
    IQSpecializationIndexService qSpecializationIndexService
) : ControllerBase
{
    [HttpGet]
    [Route("indexes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIndexesAsync()
    {
        var indexes = await qSpecializationIndexService.GetAllIndexes();
        return this.Ok(indexes);
    }

    [HttpPost]
    [Route("indexes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> SaveIndex([FromForm] QSpecializationIndexCreate indexCreate)
    {
        try
        {
            var index = await qSpecializationIndexService.SaveIndex(indexCreate);
            var specializationResponse = new QSpecializationIndexResponse(index);

            return this.Ok(specializationResponse);
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Index creation threw an exception");

            return this.StatusCode(500, "Failed to create index.");
        }
    }

    [HttpPatch]
    [Route("indexes/{indexId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditIndex(
        [FromForm] QSpecializationIndexMutate qIndexMutate,
        [FromRoute] Guid indexId
    )
    {
        try
        {
            var indexToEdit = await qSpecializationIndexService.UpdateIndex(indexId, qIndexMutate);
            if (indexToEdit != null)
            {
                return this.Ok(indexToEdit);
            }
            return this.StatusCode(500, $"Failed to update index for id '{indexId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Index update threw an exception");

            return this.StatusCode(500, $"Failed to update index for id '{indexId}'.");
        }
    }

    [HttpDelete]
    [Route("indexes/{indexId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteIndex(Guid indexId)
    {
        try
        {
            var indexToDelete = await qSpecializationIndexService.DeleteIndex(indexId);
            if (indexToDelete != null)
            {
                return this.Ok(true);
            }
            return this.StatusCode(500, $"Failed to delete index for id '{indexId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Index delete threw an exception");

            return this.StatusCode(500, $"Failed to delete index for id '{indexId}'.");
        }
    }

    [HttpPost]
    [Route("indexes/order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OrderSpecializationsAsync([FromBody] OrderMapGuidToInt qSpecializationOrder)
    {
        try
        {
            await qSpecializationIndexService.OrderSpecializations(qSpecializationOrder);
            return this.NoContent();
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Index swap order threw an exception");

            return this.StatusCode(500, $"Failed to order specializations: {ex.Message}.");
        }
    }
}
