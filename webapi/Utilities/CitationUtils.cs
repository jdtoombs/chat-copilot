using System.Globalization;
using System.IO;

namespace CopilotChat.WebApi.Utilities;

/// <summary>
/// Provides utility methods for managing and processing citations tasks.
/// </summary>
public static class CitationUtils
{
    /// <summary>
    /// Determines the MIME type of a file based on its extension.
    /// </summary>
    /// <param name="link">The path or URL to the file.</param>
    /// <returns>The MIME type corresponding to the file extension, or a default type if the extension is unknown.</returns>
    public static string GetContentType(string link)
    {
        string fileExtension = Path.GetExtension(link).TrimStart('.').ToLower(CultureInfo.CurrentCulture);
        return fileExtension switch
        {
            "pdf" => "application/pdf", // PDF files
            "doc" => "application/msword", // Microsoft Word documents
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Microsoft Word (OpenXML)
            "jpg" => "image/jpeg", // JPEG images
            "jpeg" => "image/jpeg", // JPEG images
            "png" => "image/png", // PNG images
            "gif" => "image/gif", // GIF images
            "csv" => "text/csv", // CSV files
            _ =>
                "application/octet-stream" // Default content type for unknown extensions
            ,
        };
    }
}
