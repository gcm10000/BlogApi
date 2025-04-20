namespace BlogApi.Application.Infrastructure.Minio;
public class MinioUploadResult
{
    public bool Success { get; set; }
    public string ObjectUrl { get; set; } = string.Empty;
}
