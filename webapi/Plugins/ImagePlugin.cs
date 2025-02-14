using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace CopilotChat.WebApi.Plugins;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class ImagePlugin(Kernel kernel, ITextToImageService textToImageService)
{
    [KernelFunction("generate_image")]
    [Description("Generate an image and returns a url to the hosted image.")]
    public async Task<string> GenerateImage(
        [Description("Description of the image to be generated")] string imagePrompt,
        CancellationToken cancellationToken = default
    ) => await textToImageService.GenerateImageAsync(imagePrompt, 1024, 1024, kernel, cancellationToken);
}
