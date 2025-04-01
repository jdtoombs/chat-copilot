// Copyright (c) Quartech. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Storage;
using CopilotChat.WebApi.Plugins.Chat.Ext;
using CopilotChat.WebApi.Storage;
using CopilotChat.WebApi.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// The implementation class for specialization service.
/// </summary>
public class SpecializationService(
    SpecializationRepository specializationSourceRepository,
    IOptions<QAzureOpenAIChatOptions> qAzureOpenAIChatOptions,
    IBlobStorage blobStorage
) : ISpecializationService
{
    public Task<IEnumerable<Specialization>> GetAllSpecializations() =>
        specializationSourceRepository.GetAllSpecializationsAsync();

    public Task<Specialization> GetSpecializationAsync(string id) =>
        specializationSourceRepository.GetSpecializationAsync(id);

    public Task<Specialization> GetDefaultSpecialization() => specializationSourceRepository.GetDefaultSpecialization();

    public async Task<Specialization> SaveSpecialization(Specialization specialization)
    {
        var entity = specialization with
        {
            Id = Guid.NewGuid().ToString(),
            ImageFilePath = ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationImage),
            IconFilePath = ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationIcon),
        };

        this.Validate(entity);

        await specializationSourceRepository.CreateAsync(entity);

        return entity;
    }

    public Task UpdateSpecialization(Specialization specialization) =>
        specializationSourceRepository.UpsertAsync(specialization);

    public async Task UpdateIcon(Specialization specialization, IFormFile icon)
    {
        specialization.IconFilePath = await this.UpsertSpecializationBlobAsync(
            icon,
            specialization.IconFilePath,
            false,
            ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationIcon)
        );

        await specializationSourceRepository.UpsertAsync(specialization);
    }

    public async Task DeleteIcon(Specialization specialization)
    {
        specialization.IconFilePath = await this.UpsertSpecializationBlobAsync(
            null,
            specialization.IconFilePath,
            true,
            ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationIcon)
        );

        await specializationSourceRepository.UpsertAsync(specialization);
    }

    public async Task UpdateImage(Specialization specialization, IFormFile image)
    {
        specialization.ImageFilePath = await this.UpsertSpecializationBlobAsync(
            image,
            specialization.ImageFilePath,
            false,
            ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationIcon)
        );

        await specializationSourceRepository.UpsertAsync(specialization);
    }

    public async Task DeleteImage(Specialization specialization)
    {
        specialization.ImageFilePath = await this.UpsertSpecializationBlobAsync(
            null,
            specialization.ImageFilePath,
            true,
            ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationImage)
        );

        await specializationSourceRepository.UpsertAsync(specialization);
    }

    public async Task<bool> DeleteSpecialization(Guid specializationId)
    {
        Specialization? specializationToDelete = await specializationSourceRepository.FindByIdAsync(
            specializationId.ToString()
        );

        await specializationSourceRepository.DeleteAsync(specializationToDelete);

        // Attempt to create URIs for image and icon
        if (
            Uri.TryCreate(specializationToDelete.ImageFilePath, UriKind.Absolute, out var imageFileUri)
            && Uri.TryCreate(specializationToDelete.IconFilePath, UriKind.Absolute, out var iconFileUri)
        )
        {
            // Delete image file from blob storage if it exists
            if (await blobStorage.BlobExistsAsync(imageFileUri))
            {
                await blobStorage.DeleteBlobByURIAsync(imageFileUri);
            }

            // Delete icon file from blob storage if it exists
            if (await blobStorage.BlobExistsAsync(iconFileUri))
            {
                await blobStorage.DeleteBlobByURIAsync(iconFileUri);
            }
        }
        return true;
    }

    public async Task OrderSpecializations(OrderMapGuidToInt specializationOrder)
    {
        if (specializationOrder == null)
        {
            throw new ArgumentNullException(nameof(specializationOrder), "SpecializationOrder must be provided.");
        }

        var specializations = (await this.GetAllSpecializations()).ToList();

        var upsertTasks = new List<Task>();

        foreach (var order in specializationOrder.Ordering)
        {
            string specId = order.Key;
            int newOrder = order.Value;

            var specialization = specializations.FirstOrDefault(s => s.Id == specId);
            if (specialization != null)
            {
                // Update the order
                specialization.Order = newOrder;
                upsertTasks.Add(specializationSourceRepository.UpsertAsync(specialization));
            }
        }
        await Task.WhenAll(upsertTasks);
    }

    /// <summary>
    /// Upsert the specialization blob and return filepath or blob storage URI.
    /// </summary>
    /// <param name="file">File to store in blob storage</param>
    /// <param name="fileUriString">File path URI</param>
    /// <param name="delete">Flag to delete the file from the blob storage</param>
    /// <param name="filePathDefault">File path default value</param>
    /// <returns>FilePath or Blob Storage URI</returns>
    private async Task<string> UpsertSpecializationBlobAsync(
        IFormFile? file,
        string fileUriString,
        bool delete = false,
        string filePathDefault = ""
    )
    {
        bool uriIsValid = Uri.TryCreate(fileUriString, UriKind.Absolute, out Uri? fileUri);

        // If the URI is not valid, return the default path immediately
        if (!uriIsValid || fileUri == null)
        {
            return filePathDefault;
        }

        var blobExists = await blobStorage.BlobExistsAsync(fileUri);

        // 1. File provided and a default file path is stored in the DB
        if (file != null && !blobExists)
        {
            return await blobStorage.AddBlobAsync(file);
        }

        // 2. File provided and a Blob Storage URI is stored in the DB
        if (file != null && blobExists)
        {
            await blobStorage.DeleteBlobByURIAsync(fileUri);
            return await blobStorage.AddBlobAsync(file);
        }

        // 3. File not provided and a default file path is stored in the DB and delete flag is set
        if (file == null && blobExists && delete)
        {
            await blobStorage.DeleteBlobByURIAsync(fileUri);

            return filePathDefault;
        }

        return fileUriString;
    }

    private void Validate(Specialization completionDeploymentModel)
    {
        if (!string.IsNullOrWhiteSpace(completionDeploymentModel.IndexId))
        {
            Assert.AreNotEqual(completionDeploymentModel.Strictness, null);
            Assert.AreNotEqual(completionDeploymentModel.DocumentCount, null);
            Assert.AreNotEqual(completionDeploymentModel.MaxResponseTokenLimit, null);
            Assert.AreNotEqual(completionDeploymentModel.PastMessagesIncludedCount, null);
        }
    }
}
