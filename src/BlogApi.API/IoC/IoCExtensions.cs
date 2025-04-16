using BlogApi.API.Services;
using BlogApi.Application.Infrastructure.Identity.Configurations;
using BlogApi.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BlogApi.API.IoC;

public static class IoCExtensions
{
    public static IServiceCollection ConfigureWebService(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtOptions));
        var securityKeyFromConfiguration = jwtAppSettingOptions["SecurityKey"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyFromConfiguration!));

        serviceCollection.AddScoped<ICurrentUserService, CurrentUserService>();
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = true, // Desativa a validação da audiência
                ValidateLifetime = true, // Pode ajustar de acordo com suas necessidades
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtOptions:Issuer"],
                ValidAudience = configuration["JwtOptions:Audience"],
                IssuerSigningKey = securityKey
            };
        });
        //serviceCollection.AddAuthorizationBuilder();
        //serviceCollection.AddIdentityApiEndpoints<ApplicationUser>();
        serviceCollection.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        serviceCollection.AddHttpContextAccessor();

        serviceCollection.AddSwaggerGen(setup =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "JWT Bearer Token",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        serviceCollection.AddCors(policyBuilder =>
            policyBuilder.AddDefaultPolicy(policy =>
            {
                var ASPNETCORE_ENVIRONMENT = configuration["ASPNETCORE_ENVIRONMENT"];

                if (ASPNETCORE_ENVIRONMENT == Environments.Development)
                {
                    policy
                        .SetIsOriginAllowed(_ => true) // aceita qualquer origem
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // isso permite cookies, auth headers, etc.
                }
                else
                {
                    var FRONT_END_URL = configuration["ENVIRONMENT_VARIABLE_FRONT_END_URL"];
                    policy.WithOrigins(FRONT_END_URL!)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // idem
                }
            })
        );



        return serviceCollection;
    }

}
