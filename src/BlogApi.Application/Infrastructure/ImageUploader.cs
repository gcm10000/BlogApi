using BlogApi.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BlogApi.Application.Infrastructure;

public static class ImageUploader
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public static async Task<string?> SaveImageAsync(IFormFile? imageFile, string? imageUrl, string uploadPath)
    {
        if (imageFile != null)
        {
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                throw new BusinessRuleException("Extensão de imagem não suportada.");

            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl.StartsWith("http"))
        {
            return imageUrl;
        }

        return null;
    }
}
