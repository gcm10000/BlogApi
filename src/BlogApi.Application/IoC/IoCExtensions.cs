using BlogApi.Application.Infrastructure.Identity.Services;
using BlogApi.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlogApi.Application.IoC;

public static class IoCExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection serviceCollection)
    {

        var assembly = Assembly.GetExecutingAssembly();
        serviceCollection.AddMediatR(configuration =>
        {
            //configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            //configuration.AddOpenBehavior(typeof(CacheBehaviour<,>));
            configuration.RegisterServicesFromAssembly(assembly);
        });

        //serviceCollection.AddValidatorsFromAssembly(assembly);
        serviceCollection.AddScoped<ITokenService, TokenService>();

        return serviceCollection;
    }
}