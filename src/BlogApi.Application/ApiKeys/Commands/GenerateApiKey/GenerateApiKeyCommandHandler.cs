using BlogApi.Application.ApiKeys.Dto;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Application.Infrastructure.Identity.Services;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BlogApi.Application.ApiKeys.Commands.GenerateApiKey;
public class GenerateApiKeyCommandHandler : IRequestHandler<GenerateApiKeyCommand, GenerateApiKeyResult>
{
    private readonly IdentityDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly CreateApiKeyService _createApiKeyService;

    public GenerateApiKeyCommandHandler(
        IdentityDbContext context, 
        ICurrentUserService currentUserService,
        CreateApiKeyService createApiKeyService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _createApiKeyService = createApiKeyService;
    }

    public async Task<GenerateApiKeyResult> Handle(GenerateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var tenancyDomainId = _currentUserService.GetCurrentTenancyDomainId();

        var checkApiKeyName = await _context.ApiKeys
            .Where(x => x.TenancyDomainId  == tenancyDomainId)
            .Where(x => x.IsActive)
            .Where(x => x.Name == request.Name)
            .AnyAsync();

        if (checkApiKeyName)
        {
            throw new BusinessRuleException($"Não foi possível criar a chave de acesso: o nome '{request.Name}' já está em uso. Por favor, escolha um nome diferente.");
        }

        //// REALIZAR O CASAMENTO DO TOKEN Bearer do header 'Authorization' OU 'X-API-KEY' com a rota, sempre!

        //// Criar um modal para quem estiver no MainTenancy como Administrador,
        //// exibir select + search para definir qual Tenancy gostaria de acessar

        //// Criar uma ApiKey DEFAULT (que não pode ser apagada)
        //// para realizar leitura de post:getposts e post:getpostsbyid no momento de criar um novo TENANCY OK

        //// Criar cache de 24h na duração de ApiKey DEFAULT OK


        //// V2 -> criar seção de RSS --- https://chatgpt.com/c/6802d8f6-a34c-800b-b0e8-8d6c406d6e9d
        //// https://chatgpt.com/c/6802d8f6-a34c-800b-b0e8-8d6c406d6e9d#:~:text=%22FeedLink%22%3A%20%22https%3A//news.google.com/rss/search%3Fq%3Dbitcoin%22%2C

        //// Comprar cursos de SEO

        //if (!request.Scopes.Any())
        //{
        //    throw new BusinessRuleException("A requisição deve conter pelo menos um escopo de acesso.");
        //}

        //// Gera uma chave segura
        ////var rawKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        //// Busca os escopos existentes com base nos nomes
        //var scopes = await _context.ApiScopes
        //    .Where(s => request.Scopes.Contains(s.Name))
        //    .ToListAsync(cancellationToken);

        //if (scopes.Count != request.Scopes.Count)
        //{
        //    var notFound = request.Scopes.Except(scopes.Select(s => s.Name));
        //    throw new BusinessRuleException($"Os seguintes escopos não foram encontrados: {string.Join(", ", notFound)}");
        //}

        //var apiKey = new ApiKey
        //{
        //    Name = request.Name,
        //    Key = rawKey,
        //    IsProtected = false,
        //    TenancyDomainId = tenancyDomainId,
        //    ApiKeyScopes = scopes.Select(scope => new ApiKeyScope
        //    {
        //        ApiScopeId = scope.Id
        //    }).ToList()
        //};

        //_context.ApiKeys.Add(apiKey);
        //await _context.SaveChangesAsync(cancellationToken);

        var apiKey = await _createApiKeyService.GenerateApiKeyAsync
        (
            request.Name,
            [.. request.Scopes],
            tenancyDomainId, 
            isProtected: false,
            cancellationToken
        );


        return new GenerateApiKeyResult(
             apiKey.Name,
             apiKey.Key,
             apiKey.TenancyDomainId,
             apiKey.ApiKeyScopes.Select(s => s.ApiScope.Name).ToList(),
             apiKey.CreatedAt
         );
    }

    //private static string GenerateSecureKey()
    //{
    //    var keyBytes = RandomNumberGenerator.GetBytes(32);
    //    return Convert.ToBase64String(keyBytes);
    //}
}
