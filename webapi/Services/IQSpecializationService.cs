// Copyright (c) Quartech. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;
using Microsoft.AspNetCore.Http;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// Defines specialization service
/// </summary>
public interface IQSpecializationService
{
    /// <summary>
    /// Retrieve all specializations.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains all specializations</returns>
    Task<IEnumerable<Specialization>> GetAllSpecializations();

    /// <summary>
    /// Retrieve a specialization based on id.
    /// </summary>
    /// <param name="id">Specialization id</param>
    /// <returns>Returns the specialization</returns>
    Task<Specialization> GetSpecializationAsync(string id);

    /// <summary>
    /// Creates new specialization.
    /// </summary>
    /// <param name="qSpecializationMutate">Specialization mutate payload</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the specialization</returns>
    Task<Specialization> SaveSpecialization(QSpecializationMutate qSpecializationMutate);

    /// <summary>
    /// Updates the specialization.
    /// </summary>
    /// <param name="specialization">Specialization to update</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateSpecialization(Specialization specialization);

    /// <summary>
    /// Updates the specialization.
    /// </summary>
    /// <param name="specializationId">Unique identifier of the specialization</param>
    /// <param name="qSpecializationMutate">Specialization mutate payload</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the specialization</returns>
    Task<Specialization?> UpdateSpecialization(Guid specializationId, QSpecializationMutate qSpecializationMutate);

    /// <summary>
    /// Update specialization icon
    /// </summary>
    /// <param name="specialization">Specialization to update</param>
    /// <param name="icon">Image file to save as specialization icon</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateIcon(Specialization specialization, IFormFile icon);

    /// <summary>
    /// Delete specialization icon
    /// </summary>
    /// <param name="specialization">Specialization to update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteIcon(Specialization specialization);

    /// <summary>
    /// Update specialization image
    /// </summary>
    /// <param name="specialization">Specialization to update</param>
    /// <param name="icon">Image file to save as specialization image</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateImage(Specialization specialization, IFormFile image);

    /// <summary>
    /// Delete specialization image
    /// </summary>
    /// <param name="specialization">Specialization to update</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteImage(Specialization specialization);

    /// <summary>
    /// Deletes the specialization.
    /// </summary>
    /// <param name="specializationId">Unique identifier of the specialization</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the delete state</returns>
    Task<bool> DeleteSpecialization(Guid specializationId);

    Task OrderSpecializations(OrderMapGuidToInt specializationOrder);
}
