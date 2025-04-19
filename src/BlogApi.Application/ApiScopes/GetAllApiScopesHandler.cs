using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.ApiScopes;
public class GetAllApiScopesHandler : IRequestHandler<GetAllApiScopesQuery, List<ApiScope>>
{
    private readonly IdentityDbContext _context;

    public GetAllApiScopesHandler(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApiScope>> Handle(GetAllApiScopesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ApiScopes
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }
}