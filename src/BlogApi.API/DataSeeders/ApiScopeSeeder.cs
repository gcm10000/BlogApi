using BlogApi.API.Attributes;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlogApi.API.DataSeeders;

public class ApiScopeSeeder
{
    private readonly IdentityDbContext _context;

    public ApiScopeSeeder(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(Assembly[] assembliesToScan)
    {
        var scopesFromAttributes = new HashSet<string>();

        foreach (var assembly in assembliesToScan)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                // Check class-level attributes
                var classAttributes = type.GetCustomAttributes<RequireApiScopeAttribute>(inherit: true);
                foreach (var attr in classAttributes)
                    scopesFromAttributes.Add(attr.Scope);

                // Check method-level attributes
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var methodAttributes = method.GetCustomAttributes<RequireApiScopeAttribute>(inherit: true);
                    foreach (var attr in methodAttributes)
                        scopesFromAttributes.Add(attr.Scope);
                }
            }
        }

        // Fetch current scopes from DB
        var existingScopes = await _context.Set<ApiScope>()
            .Select(s => s.Name)
            .ToListAsync();

        var scopesToAdd = scopesFromAttributes
            .Except(existingScopes)
            .Select(scope => new ApiScope { Name = scope })
            .ToList();

        if (scopesToAdd.Any())
        {
            _context.Set<ApiScope>().AddRange(scopesToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
