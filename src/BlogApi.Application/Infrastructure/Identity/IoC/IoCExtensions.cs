using BlogApi.Application.Constants;
using BlogApi.Application.Infrastructure.Identity.Configurations;
using BlogApi.Application.Infrastructure.Identity.DataSeeders;
using BlogApi.Application.Infrastructure.Identity.Services;
using BlogApi.Application.Interfaces;
using BlogApi.Infrastructure.Identity.Data;
using BlogApi.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogApi.Infrastructure.Identity.IoC;

public static class IoCExtensions
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
            var connectionString = configuration.GetConnectionString("IDENTITY_DATABASE") ?? configuration["IDENTITY_DATABASE"];
            options.UseMySql(connectionString!, serverVersion);
        });

        services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<RoleSeeder>();
        services.AddScoped<IAuthService, AuthService>();

        // Chamada do ConfigureAuthentication
        services.ConfigureAuthentication(configuration);

        return services;
    }

    private static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtOptions));
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!));
        var inviteTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["InviteTokenSecurityKey"]!));

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSettings[nameof(JwtOptions.Issuer)];
            options.Audience = jwtSettings[nameof(JwtOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            options.InviteTokenSigningCredentials = new SigningCredentials(inviteTokenKey, SecurityAlgorithms.HmacSha256);
            options.Expiration = int.Parse(jwtSettings[nameof(JwtOptions.Expiration)] ?? "0");
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.AllowedForNewUsers = false;
        });

        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = jwtSettings["Issuer"],
        //        ValidAudience = jwtSettings["Audience"],
        //        IssuerSigningKey = securityKey
        //    };
        //});

        services.AddAuthorization(options =>
        {
            options.AddPolicy(RoleConstants.Administrator, policy =>
                policy.RequireRole(RoleConstants.Administrator));

            options.AddPolicy(RoleConstants.Author, policy =>
                policy.RequireRole(RoleConstants.Author));

            options.AddPolicy(RoleConstants.AdministratorAndAuthor, policy =>
                policy.RequireRole(RoleConstants.AdministratorAndAuthor));
        });

        return services;
    }
}

