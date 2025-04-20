namespace BlogApi.Application.Infrastructure.Minio;
public class MinioSettings
{
    public string Endpoint { get; set; } = "http://localhost:9000";
    public string AccessKey { get; set; } = "admin";
    public string SecretKey { get; set; } = "admin123";
    public string Region { get; set; } = "us-east-1"; // pode ser qualquer região
}
