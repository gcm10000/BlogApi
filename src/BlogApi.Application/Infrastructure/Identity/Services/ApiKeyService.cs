using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Dtos;
using BlogApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Infrastructure.Identity.Services;
public class ApiKeyService : IApiKeyService
{
    private readonly IdentityDbContext _context;

    public ApiKeyService(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ApiKeyValidationResultDto> GetApiKeyAsync(string apiKeyValue)
    {
        var apiKey = await _context.ApiKeys
            .Include(k => k.ApiKeyScopes)
                .ThenInclude(ks => ks.ApiScope)
            .FirstOrDefaultAsync(k => k.Key == apiKeyValue && k.IsActive);

        if (apiKey == null)
        {
            return new ApiKeyValidationResultDto
            {
                IsValid = false,
                ErrorMessage = "API Key inválida"
            };
        }
        

        return new ApiKeyValidationResultDto
        {
            IsValid = true,
            ApiKeyId = apiKey.Id,
            TenancyDomainId = apiKey.TenancyDomainId,
            Name = apiKey.Name,
            Scopes = apiKey.ApiKeyScopes.Select(s => s.ApiScope.Name).ToList()
        };
    }
}
