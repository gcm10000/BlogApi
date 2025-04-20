using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel.Args;


namespace BlogApi.Application.Infrastructure.Minio.IoC;
public static class IoCExtensions
{
    public static IServiceCollection ConfigureMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<MinioStorageService>();

        return services;
    }
}
