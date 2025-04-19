using MediatR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Interfaces;
using BlogApi.Application.ApiKeys.Dto;

namespace BlogApi.Application.ApiKeys.Queries.ListApiKeys;

public class ListApiKeysQueryHandler : IRequestHandler<ListApiKeysQuery, List<ApiKeyDto>>
{
    private readonly IdentityDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ListApiKeysQueryHandler(IdentityDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ApiKeyDto>> Handle(ListApiKeysQuery request, CancellationToken cancellationToken)
    {
        var tenancyDomainId = _currentUserService.GetCurrentTenancyDomainId();

        var keys = await _context.ApiKeys
            .Where(k => k.TenancyDomainId == tenancyDomainId)
            .Include(k => k.ApiKeyScopes)
                .ThenInclude(ks => ks.ApiScope)
            .Where(k => k.Name.Contains(request.Name ?? ""))
            .Select(k => new ApiKeyDto(
                k.Id,
                k.Name,
                k.Key.Length > 6 ? k.Key.Substring(0, 20) + "..." : k.Key,
                k.CreatedAt,
                k.IsActive,
                k.ApiKeyScopes.Select(s => s.ApiScope).ToList()
            ))
            .ToListAsync(cancellationToken);

        return keys;
    }
}
