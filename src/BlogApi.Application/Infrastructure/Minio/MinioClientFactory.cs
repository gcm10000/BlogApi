using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace BlogApi.Application.Infrastructure.Minio;

public class MinioClientFactory
{
    private readonly MinioSettings _settings;

    public MinioClientFactory(MinioSettings settings)
    {
        _settings = settings;
    }

    public IAmazonS3 CreateClient()
    {
        return new AmazonS3Client(
            _settings.AccessKey,
            _settings.SecretKey,
            new AmazonS3Config
            {
                ServiceURL = _settings.Endpoint,
                ForcePathStyle = true, // ESSENCIAL para funcionar com MinIO
                RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region),
                UseHttp = _settings.Endpoint.StartsWith("http://")
            }
        );
    }
}
