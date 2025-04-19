using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Infrastructure.Identity.Services;
public class CreateApiKeyService
{
    private readonly IdentityDbContext _context;

    public CreateApiKeyService(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ApiKey> GenerateApiKeyAsync(
        string name, 
        string[] scopes, 
        int tenancyDomainId,
        bool isProtected,
        CancellationToken cancellationToken)
    {
        if (!scopes.Any())
        {
            throw new BusinessRuleException("A requisição deve conter pelo menos um escopo de acesso.");
        }

        // Verifica se o nome da chave de API já está em uso
        var checkApiKeyName = await _context.ApiKeys
            .Where(x => x.TenancyDomainId == tenancyDomainId)
            .Where(x => x.IsActive)
            .Where(x => x.Name == name)
            .AnyAsync(cancellationToken);

        if (checkApiKeyName)
        {
            throw new BusinessRuleException($"Não foi possível criar a chave de acesso: o nome '{name}' já está em uso. Por favor, escolha um nome diferente.");
        }

        // Gera uma chave segura
        var rawKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        // Busca os escopos existentes com base nos nomes
        var scopesList = await _context.ApiScopes
            .Where(s => scopes.Contains(s.Name))
            .ToListAsync(cancellationToken);

        if (scopesList.Count != scopes.Length)
        {
            var notFound = scopes.Except(scopesList.Select(s => s.Name));
            throw new BusinessRuleException($"Os seguintes escopos não foram encontrados: {string.Join(", ", notFound)}");
        }

        var apiKey = new ApiKey
        {
            Name = name,
            Key = rawKey,
            IsProtected = isProtected,
            TenancyDomainId = tenancyDomainId,
            ApiKeyScopes = scopesList.Select(scope => new ApiKeyScope
            {
                ApiScopeId = scope.Id
            }).ToList()
        };

        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync(cancellationToken);

        return apiKey;
    }

}
