using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace CopilotChat.WebApi.Plugins;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class ImagePlugin(Kernel kernel)
{
    [KernelFunction("generate_image")]
    [Description("Generate an image and returns a url to the hosted image.")]
    public async Task<string> GenerateImage(
        KernelArguments context,
        [Description("Description of the image to be generated")] string imagePrompt,
        [Description("Determine if permission has been granted to this system persona to generate images")]
            bool canGenerate = false,
        CancellationToken cancellationToken = default
    )
    {
        if (!canGenerate)
        {
            return "Sorry you do not have permissions to generate images";
        }

        var provider = kernel.GetRequiredService<IServiceProvider>();
        var textToImageService = provider.GetRequiredService<ITextToImageService>();

        if (textToImageService == null)
        {
            return "Sorry, no text to image service has been configured";
        }

        return await textToImageService.GenerateImageAsync(imagePrompt, 1024, 1024, kernel, cancellationToken);
    }
}
