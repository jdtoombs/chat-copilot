using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CopilotChat.WebApi.Services;

internal interface IQBlobStorage
{
    Task<bool> BlobExistsAsync(System.Uri blobURI);

    Task<string> AddBlobAsync(IFormFile blob);

    Task DeleteBlobByURIAsync(System.Uri blobURI);
}
