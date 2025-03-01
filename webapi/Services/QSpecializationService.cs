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

namespace CopilotChat.WebApi.Services;

/// <summary>
/// The implementation class for specialization service.
/// </summary>
public class QSpecializationService(
    SpecializationRepository specializationSourceRepository,
    IOptions<QAzureOpenAIChatOptions> qAzureOpenAIChatOptions,
    IQBlobStorage qBlobStorage
) : IQSpecializationService
{
    /// <summary>
    /// Retrieve all specializations.
    /// </summary>
    /// <returns>The task result contains all specializations</returns>
    public Task<IEnumerable<Specialization>> GetAllSpecializations()
    {
        return specializationSourceRepository.GetAllSpecializationsAsync();
    }

    /// <summary>
    /// Retrieve a specialization based on key.
    /// </summary>
    /// <param name="key">Specialization key</param>
    /// <returns>Returns the specialization source</returns>
    public Task<Specialization> GetSpecializationAsync(string id)
    {
        return specializationSourceRepository.GetSpecializationAsync(id);
    }

    /// <summary>
    /// Creates new specialization.
    /// </summary>
    /// <param name="qSpecializationMutate">Specialization mutate payload</param>
    /// <returns>The task result contains the specialization source</returns>
    public async Task<Specialization> SaveSpecialization(QSpecializationMutate qSpecializationMutate)
    {
        // Add the image to the blob storage or use the default image
        var imageFilePath =
            qSpecializationMutate.ImageFile == null
                ? ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationImage)
                : await qBlobStorage.AddBlobAsync(qSpecializationMutate.ImageFile);

        // Add the icon to the blob storage or use the default icon
        var iconFilePath =
            qSpecializationMutate.IconFile == null
                ? ResourceUtils.GetImageAsDataUri(qAzureOpenAIChatOptions.Value.DefaultSpecializationIcon)
                : await qBlobStorage.AddBlobAsync(qSpecializationMutate.IconFile);

        var deserializedSuggestions = qSpecializationMutate.Suggestions;

        Specialization specializationSource = new(
            Label: qSpecializationMutate.Label,
            Name: qSpecializationMutate.Name,
            Description: qSpecializationMutate.Description,
            RoleInformation: qSpecializationMutate.RoleInformation,
            InitialChatMessage: qSpecializationMutate.InitialChatMessage,
            OpenAIDeploymentId: qSpecializationMutate.OpenAIDeploymentId,
            CompletionDeploymentName: qSpecializationMutate.CompletionDeploymentName,
            IndexId: qSpecializationMutate.IndexId,
            IsDefault: qSpecializationMutate.IsDefault,
            RestrictResultScope: qSpecializationMutate.IndexId != null
                ? qSpecializationMutate.RestrictResultScope
                : null,
            Strictness: qSpecializationMutate.IndexId != null ? qSpecializationMutate.Strictness : null,
            DocumentCount: qSpecializationMutate.IndexId != null ? qSpecializationMutate.DocumentCount : null,
            PastMessagesIncludedCount: qSpecializationMutate.IndexId != null
                ? qSpecializationMutate.PastMessagesIncludedCount
                : null,
            MaxResponseTokenLimit: qSpecializationMutate.IndexId != null
                ? qSpecializationMutate.MaxResponseTokenLimit
                : null,
            ImageFilePath: imageFilePath,
            IconFilePath: iconFilePath,
            GroupMemberships: qSpecializationMutate.GroupMemberships,
            Order: qSpecializationMutate.Order,
            Suggestions: deserializedSuggestions != null ? deserializedSuggestions : new List<string>(),
            CanGenImages: qSpecializationMutate.CanGenImages
        );

        await specializationSourceRepository.CreateAsync(specializationSource);

        return specializationSource;
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

    /// <summary>
    /// Deletes the specialization.
    /// </summary>
    /// <param name="specializationId">Unique identifier of the specialization</param>
    /// <returns>The task result contains the delete state</returns>
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
            if (await qBlobStorage.BlobExistsAsync(imageFileUri))
            {
                await qBlobStorage.DeleteBlobByURIAsync(imageFileUri);
            }

            // Delete icon file from blob storage if it exists
            if (await qBlobStorage.BlobExistsAsync(iconFileUri))
            {
                await qBlobStorage.DeleteBlobByURIAsync(iconFileUri);
            }
        }
        return true;
    }

    /// <summary>
    /// Reorders specializations based on the provided ordering information. This method updates the order of existing specializations
    /// in the database asynchronously, utilizing concurrent task execution for efficiency.
    /// </summary>
    /// <param name="specializationOrder">A QSpecializationOrder object containing the new order for specializations, where each key is a specialization ID and each value is the intended order.</param>
    /// <returns>A Task representing the asynchronous operation of updating all relevant specializations.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="specializationOrder"/> is null.</exception>
    public async Task OrderSpecializations(OrderMapGuidToInt specializationOrder)
    {
        if (specializationOrder == null)
        {
            throw new ArgumentNullException(nameof(specializationOrder), "QSpecializationOrder must be provided.");
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

        var blobExists = await qBlobStorage.BlobExistsAsync(fileUri);

        // 1. File provided and a default file path is stored in the DB
        if (file != null && !blobExists)
        {
            return await qBlobStorage.AddBlobAsync(file);
        }

        // 2. File provided and a Blob Storage URI is stored in the DB
        if (file != null && blobExists)
        {
            await qBlobStorage.DeleteBlobByURIAsync(fileUri);
            return await qBlobStorage.AddBlobAsync(file);
        }

        // 3. File not provided and a default file path is stored in the DB and delete flag is set
        if (file == null && blobExists && delete)
        {
            await qBlobStorage.DeleteBlobByURIAsync(fileUri);

            return filePathDefault;
        }

        return fileUriString;
    }
}
