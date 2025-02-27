using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace CopilotChat.WebApi.Services;

/// <summary>
/// The implementation class for Blob Storage.
/// </summary>
internal class QBlobStorage(BlobContainerClient blobContainerClient) : IQBlobStorage
{
    /// <summary>
    /// Checks if the provided URI points to a valid Blob Storage File
    /// </summary>
    /// <param name="blobURI">Blob Storage URI</param>
    /// <returns>Boolean indicator if string is a URI</returns>
    public async Task<bool> BlobExistsAsync(System.Uri blobURI)
    {
        try
        {
            BlobUriBuilder blobUriBuilder = new(blobURI);
            var blobClient = blobContainerClient.GetBlobClient(blobUriBuilder.BlobName);
            return await blobClient.ExistsAsync();
        }
        catch (Azure.RequestFailedException)
        {
            return false;
        }
    }

    /// <summary>
    /// Add a blob to the storage container
    /// </summary>
    /// <param name="blob">Blob file</param>
    /// <returns>Blob Storage URI identifier</returns>
    public async Task<string> AddBlobAsync(IFormFile blob)
    {
        var blobClient = blobContainerClient.GetBlobClient(
            // Inject a unique identifier into the blob file name ie: file_name$1000-1000-1000-1000.ext
            blob.FileName.Insert(blob.FileName.LastIndexOf('.'), "$" + Guid.NewGuid().ToString())
        );

        await blobClient.UploadAsync(blob.OpenReadStream(), true);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Remove a blob from the storage container by URI
    /// </summary>
    /// <param name="blobURI">Blob Storage URI</param>
    public async Task DeleteBlobByURIAsync(System.Uri blobURI)
    {
        BlobUriBuilder blobUriBuilder = new(blobURI);

        var blobClient = blobContainerClient.GetBlobClient(blobUriBuilder.BlobName);

        await blobClient.DeleteIfExistsAsync();
    }
}
