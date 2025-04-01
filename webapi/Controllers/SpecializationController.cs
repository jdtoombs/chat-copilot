// Copyright (c) Quartech. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CopilotChat.WebApi.Auth;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CopilotChat.WebApi.Controllers;

/// <summary>
/// Controller responsible for managing specializations.
/// </summary>
[ApiController]
public class SpecializationController(
    ILogger<SpecializationController> logger,
    ISpecializationService specializationService,
    ICompletionDeploymentModelService completionDeploymentModelService
) : ControllerBase
{
    /// <summary>
    /// Get a specialization
    /// </summary>
    [HttpGet]
    [Route("specializations/{specializationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpecialization([FromRoute] Guid specializationId, IMapper mapper)
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());
        var deploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specializationId.ToString()
        );

        var response = mapper.Map<SpecializationResponse>(specialization);
        mapper.Map(deploymentModel, response);

        return this.Ok(response);
    }

    /// <summary>
    /// Get all available specializations maintained in the system.
    /// </summary>
    /// <returns>A list of available specializations. An empty list if no specializations are found.</returns>
    [HttpGet]
    [Route("specializations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<OkObjectResult> GetAllSpecializations(IMapper mapper)
    {
        var specializations = await specializationService.GetAllSpecializations();

        var specializationResponses = mapper.Map<SpecializationReadModel[]>(specializations);

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
    /// <param name="specialization">Contains the specialization parameters</param>
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
        [FromBody] SpecializationWriteModel model,
        IMapper mapper
    )
    {
        var specialization = await specializationService.SaveSpecialization(mapper.Map<Specialization>(model));

        await completionDeploymentModelService.Save(mapper.Map<CompletionDeploymentModel>(model), specialization.Id);

        return this.Ok(specialization);
    }

    /// <summary>
    /// Edit a specialization.
    /// </summary>
    /// <param name="patchSpecialization">Contains specialization patch actions</param>
    /// <param name="specializationId">The specializtion id.</param>
    /// <returns>The HTTP action result.</returns>
    [HttpPatch]
    [Route("specializations/{specializationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchSpecializationAsync(
        [FromBody] JsonPatchDocument<SpecializationWriteModel> patchSpecialization,
        [FromRoute] Guid specializationId,
        IMapper mapper
    )
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());
        var completionDeploymentModel = await completionDeploymentModelService.FindBySpecializationId(
            specializationId.ToString()
        );

        SpecializationWriteModel specializationWriteModel = new();
        mapper.Map(specialization, specializationWriteModel);
        mapper.Map(completionDeploymentModel, specializationWriteModel);

        patchSpecialization.ApplyTo(specializationWriteModel);

        await specializationService.UpdateSpecialization(mapper.Map(specializationWriteModel, specialization));

        if (completionDeploymentModel == null)
        {
            await completionDeploymentModelService.Save(
                mapper.Map<CompletionDeploymentModel>(specializationWriteModel),
                specializationId.ToString()
            );
        }
        else
        {
            await completionDeploymentModelService.Update(
                mapper.Map(specializationWriteModel, completionDeploymentModel)
            );
        }

        return this.Ok(specialization);
    }

    /// <summary>
    /// Update specialization icon
    /// </summary>
    /// <param name="icon">Image file to save as specialization icon</param>
    /// <param name="specializationId">The specializtion id</param>
    /// <returns>The HTTP action result.</returns>
    [HttpPatch]
    [Route("specializations/{specializationId:guid}/icon")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIconAsync([FromForm] IFormFile icon, [FromRoute] Guid specializationId)
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());

        await specializationService.UpdateIcon(specialization, icon);

        return this.NoContent();
    }

    /// <summary>
    /// Delete specialization icon
    /// </summary>
    /// <param name="specializationId">The specializtion id</param>
    /// <returns>The HTTP action result.</returns>
    [HttpDelete]
    [Route("specializations/{specializationId:guid}/icon")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIconAsync([FromRoute] Guid specializationId)
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());

        await specializationService.DeleteIcon(specialization);

        return this.NoContent();
    }

    /// <summary>
    /// Update specialization image
    /// </summary>
    /// <param name="image">Image file to save as specialization image</param>
    /// <param name="specializationId">The specializtion id</param>
    /// <returns>The HTTP action result.</returns>
    [HttpPatch]
    [Route("specializations/{specializationId:guid}/image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateImageAsync([FromForm] IFormFile image, [FromRoute] Guid specializationId)
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());

        await specializationService.UpdateImage(specialization, image);

        return this.NoContent();
    }

    /// <summary>
    /// Delete specialization image
    /// </summary>
    /// <param name="specializationId">The specializtion id</param>
    /// <returns>The HTTP action result.</returns>
    [HttpDelete]
    [Route("specializations/{specializationId:guid}/image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImageAsync([FromRoute] Guid specializationId)
    {
        var specialization = await specializationService.GetSpecializationAsync(specializationId.ToString());

        await specializationService.DeleteImage(specialization);

        return this.NoContent();
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
            Specialization specialization = await specializationService.GetSpecializationAsync(
                specializationId.ToString()
            );

            bool result = await specializationService.DeleteSpecialization(specializationId);

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
            await specializationService.OrderSpecializations(qSpecializationOrder);
            return this.NoContent();
        }
        catch (Azure.RequestFailedException ex)
        {
            logger.LogError(ex, "Specialization swap order threw an exception");

            return this.StatusCode(500, $"Failed to order specializations: {ex.Message}.");
        }
    }
}
