using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApi.Application.Infrastructure.Data.IoC;

public static class IoCExtensions
{
    public static IServiceCollection ConfigureData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BlogDbContext>((sp, options) =>
        {
            ConfigureDatabaseProvider(options, configuration);

            //AddInterceptors(sp, options);
        });

        return services;
    }

    //private static void AddInterceptors(IServiceProvider sp, DbContextOptionsBuilder options)
    //{
    //    options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
    //    options.AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());
    //}

    private static void ConfigureDatabaseProvider(DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dataConnectionString = configuration.GetConnectionString("DATA_DATABASE") ?? configuration["DATA_DATABASE"];

            var connectionString = dataConnectionString ?? configuration.GetConnectionString("Data");

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));

            optionsBuilder.UseMySql(connectionString, serverVersion);

        }
    }
}
