using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace BlogApi.Application.Infrastructure.Minio;
public class MinioStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(IConfiguration configuration, ILogger<MinioStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnsureBucketExistsAsync(string bucketName)
    {
        var minionSection = _configuration.GetSection("Minio");
        var endPoint = minionSection["Endpoint"];
        var port = minionSection["Port"];
        var accessKey = minionSection["AccessKey"];
        var secretKey = minionSection["SecretKey"];

        var s3Client = new MinioClient()
        .WithEndpoint(endPoint, int.Parse(port))
        .WithCredentials(accessKey, secretKey)
        .WithSSL(true) // ou true se estiver com HTTPS
        .Build();

        // Verifica se o bucket já existe
        bool found = await s3Client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!found)
        {
            await s3Client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            _logger.LogInformation("Bucket '{bucketName}' criado com sucesso.", bucketName);
        }
        else
        {
            _logger.LogError("Bucket '{bucketName}' já existe.", bucketName);
        }

    }

    public async Task<string> UploadAsync(string bucketName, IFormFile file)
    {
        var minionSection = _configuration.GetSection("Minio");
        var endPoint = minionSection["Endpoint"];
        var port = minionSection["Port"];
        var url = minionSection["Url"];
        var accessKey = minionSection["AccessKey"];
        var secretKey = minionSection["SecretKey"];

        var s3Client = new MinioClient()
        .WithEndpoint(endPoint, int.Parse(port))
        .WithCredentials(accessKey, secretKey)
        .WithSSL(true) // ou true se estiver com HTTPS
        .Build();

        var objectName = $"{Guid.NewGuid()}_{file.FileName}";

        using var stream = file.OpenReadStream();

        // Faz o upload
        await s3Client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));

        // URL pública (caso esteja com nginx/proxy configurado)
        //var endpoint = "http://localhost:9000"; // ou http://localhost:9000

        return $"{url.TrimEnd('/')}/{bucketName.Trim('/')}/{objectName}";
    }
}

