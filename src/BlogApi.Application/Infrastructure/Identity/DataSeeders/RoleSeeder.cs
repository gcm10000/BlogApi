using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlogApi.Application.Infrastructure.Identity.DataSeeders;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(RoleManager<IdentityRole> roleManager, ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await EnsureRoleExists("Administrator");
        await EnsureRoleExists("Author");
    }

    private async Task EnsureRoleExists(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                _logger.LogInformation($"Role {roleName} created successfully.");
            else
                _logger.LogError($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}