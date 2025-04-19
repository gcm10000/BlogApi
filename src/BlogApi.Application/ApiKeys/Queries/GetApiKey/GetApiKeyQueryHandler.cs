using BlogApi.Application.ApiKeys.Dto;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.ApiKeys.Queries.GetApiKey;
public class GetApiKeyQueryHandler : IRequestHandler<GetApiKeyQuery, ApiKeyDto?>
{
    private readonly IdentityDbContext _identityDbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetApiKeyQueryHandler(IdentityDbContext identityDbContext, ICurrentUserService currentUserService)
    {
        _identityDbContext = identityDbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ApiKeyDto?> Handle(GetApiKeyQuery request, CancellationToken cancellationToken)
    {
        var tenancyDomainId = _currentUserService.GetCurrentTenancyDomainId();

        var apiKey = await _identityDbContext.ApiKeys
            .Where(k => k.TenancyDomainId == tenancyDomainId)
            .Include(k => k.ApiKeyScopes)
                .ThenInclude(ks => ks.ApiScope)
            .Where(k => k.Id == request.Id)
            .Select(k => new ApiKeyDto(
                k.Id,
                k.Name,
                k.Key.Length > 6 ? k.Key.Substring(0, 20) + "..." : k.Key,
                k.CreatedAt,
                k.IsActive,
                k.ApiKeyScopes.Select(s => s.ApiScope).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return apiKey;
    }
}
