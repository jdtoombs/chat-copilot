// Copyright (c) Quartech. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Auth;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for managing specializations.
/// </summary>
[ApiController]
internal class SpecializationController(
    ILogger<SpecializationController> logger,
    IQSpecializationService qSpecializationService
) : ControllerBase
{
    /// <summary>
    /// Get all available specializations maintained in the system.
    /// </summary>
    /// <returns>A list of available specializations. An empty list if no specializations are found.</returns>
    [HttpGet]
    [Route("specializations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<OkObjectResult> GetAllSpecializations()
    {
        var specializations = await qSpecializationService.GetAllSpecializations();

        var specializationResponses = specializations.Select(s => new QSpecializationResponse(s));

        var orderedSpecializations = specializationResponses
            .Select((spec, index) => (spec, index))
            .OrderBy(x => x.spec.Order ?? x.index)
            .Select(x => x.spec)
            .ToList();

        return this.Ok(orderedSpecializations);
    }

    /// <summary>
    /// Creates a new specialization.
    /// </summary>
    /// <param name="authInfo">Auth info for the current request.</param>
    /// <param name="qSpecializationParameters">Contains the specialization parameters</param>
    /// <returns>The HTTP action result.</returns>
    [Route("specializations")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> CreateSpecializationAsync(
        [FromServices] IAuthInfo authInfo,
        [FromForm] QSpecializationMutate qSpecializationMutate
    )
    {
        try
        {
            var _specializationsource = await qSpecializationService.SaveSpecialization(qSpecializationMutate);

            QSpecializationResponse qSpecializationResponse = new(_specializationsource);
            return this.Ok(qSpecializationResponse);
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Specialization create threw an exception");

            return this.StatusCode(500, $"Failed to create specialization for label '{qSpecializationMutate.Label}'.");
        }
    }

    /// <summary>
    /// Edit a specialization.
    /// </summary>
    /// <param name="qSpecializationParameters">Contains the specialization parameters</param>
    /// <param name="specializationId">The specializtion id.</param>
    /// <returns>The HTTP action result.</returns>
    [HttpPatch]
    [Route("specializations/{specializationId:guid}/patch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditSpecializationAsync(
        [FromForm] QSpecializationMutate qSpecializationMutate,
        [FromRoute] Guid specializationId
    )
    {
        try
        {
            Specialization? specializationToEdit = await qSpecializationService.UpdateSpecialization(
                specializationId,
                qSpecializationMutate
            );

            if (specializationToEdit != null)
            {
                QSpecializationResponse qSpecializationResponse = new(specializationToEdit);
                return this.Ok(qSpecializationResponse);
            }

            return this.StatusCode(500, $"Failed to update specialization for id '{specializationId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Specialization update threw an exception");

            return this.StatusCode(500, $"Failed to edit specialization for id '{specializationId}'.");
        }
    }

    /// <summary>
    /// Delete specialization.
    /// </summary>
    /// <param name="specializationId">The specializtion id.</param>
    /// <returns>The HTTP action result.</returns>
    [HttpDelete]
    [Route("specializations/{specializationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DisableSpecializationAsync(Guid specializationId)
    {
        try
        {
            Specialization specialization = await qSpecializationService.GetSpecializationAsync(
                specializationId.ToString()
            );

            bool result = await qSpecializationService.DeleteSpecialization(specializationId);

            if (result)
            {
                return this.Ok(specializationId);
            }

            return this.StatusCode(500, $"Failed to delete specialization for id '{specializationId}'.");
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Specialization delete threw an exception");

            return this.StatusCode(500, $"Failed to delete specialization for id '{specializationId}'.");
        }
    }

    [HttpPost]
    [Route("specializations/order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OrderSpecializationsAsync([FromBody] OrderMapGuidToInt qSpecializationOrder)
    {
        try
        {
            await qSpecializationService.OrderSpecializations(qSpecializationOrder);
            return this.NoContent();
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Specialization swap order threw an exception");

            return this.StatusCode(500, $"Failed to order specializations: {ex.Message}.");
        }
    }
}
