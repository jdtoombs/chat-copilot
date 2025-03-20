using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CopilotChat.WebApi.Services;

public interface IBlobStorage
{
    Task<bool> BlobExistsAsync(System.Uri blobURI);

    Task<string> AddBlobAsync(IFormFile blob);

    Task DeleteBlobByURIAsync(System.Uri blobURI);
}
