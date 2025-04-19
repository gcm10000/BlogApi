using BlogApi.Application.Common;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Queries.GetTenancies;

public class GetTenanciesQueryHandler : IRequestHandler<GetTenanciesQuery, PagedResponse<List<TenancyDto>>>
{
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityDbContext;

    public GetTenanciesQueryHandler(BlogDbContext context, IdentityDbContext identityDbContext)
    {
        _context = context;
        _identityDbContext = identityDbContext;
    }

    public async Task<PagedResponse<List<TenancyDto>>> Handle(GetTenanciesQuery request, CancellationToken cancellationToken)
    {
        // Buscar todas as tenancies com paginação
        var query = _context.Tenancies.AsQueryable();

        // Filtro por nome
        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(t => t.Name.Contains(request.Name));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tenancies = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Busca todos os usuários e os mapeia por AuthorId
        var usersByAuthorId = await _identityDbContext.Users
            .GroupBy(u => u.AuthorId)
            .Select(g => new
            {
                AuthorId = g.Key,
                Email = g
                    .OrderBy(u => u.Id) // ou algum critério como Role == "Administrator"
                    .Select(u => u.Email)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(g => g.AuthorId, g => g.Email!, cancellationToken);

        var tenancyDtos = tenancies.Select(t => new TenancyDto
        {
            Id = t.Id,
            Name = t.Name,
            Url = t.Url,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            MainAdministratorEmail = usersByAuthorId.TryGetValue(t.MainAuthorId, out var email) ? email : string.Empty
        }).ToList();

        return new PagedResponse<List<TenancyDto>>(
            tenancyDtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}
