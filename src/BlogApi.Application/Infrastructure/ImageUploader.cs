using BlogApi.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlogApi.Application.Infrastructure;

public static class ImageUploader
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public static async Task<string?> SaveImageAsync(IFormFile? imageFile, string? imageUrl, string uploadPath)
    {
        // Iniciar o Logger (assumindo que o Logger já está configurado no projeto, ou se injetado como parâmetro)
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("ImageUploader");

        try
        {
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                logger.LogInformation("Received image file: {FileName}, Extension: {Extension}", imageFile.FileName, extension);

                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                {
                    logger.LogWarning("Unsupported image extension: {Extension} for file {FileName}", extension, imageFile.FileName);
                    throw new BusinessRuleException("Extensão de imagem não suportada.");
                }

                // Cria o diretório de upload caso não exista
                Directory.CreateDirectory(uploadPath);
                logger.LogInformation("Directory created or already exists at: {UploadPath}", uploadPath);

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(uploadPath, fileName);
                logger.LogInformation("Saving image to: {FullPath}", fullPath);

                // Salva o arquivo no sistema de arquivos
                using (var stream = new FileStream(fullPath, FileMode.CreateNew))
                {
                    await imageFile.CopyToAsync(stream);
                }

                logger.LogInformation("Image saved successfully. File path: {FilePath}", fullPath);
                return $"/uploads/{fileName}";
            }

            if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl.StartsWith("http"))
            {
                logger.LogInformation("Using external image URL: {ImageUrl}", imageUrl);
                return imageUrl;
            }

            logger.LogInformation("No image file or valid URL provided.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while uploading the image.");
            throw new BusinessRuleException("Erro ao fazer upload da imagem.");
        }
    }
}
