using BlogApi.Application.Common;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Queries.GetTenancies;

public class GetTenanciesQueryHandler : IRequestHandler<GetTenanciesQuery, PagedResponse<List<TenancyDto>>>
{
    private readonly BlogDbContext _context;

    public GetTenanciesQueryHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<List<TenancyDto>>> Handle(GetTenanciesQuery request, CancellationToken cancellationToken)
    {
        // Buscar todas as tenancies com paginação (caso solicitado)
        var query = _context.Tenancies.AsQueryable();

        // Aplicar filtros, se houver
        if (!string.IsNullOrEmpty(request.Name))
        {
            query = query.Where(t => t.Name.Contains(request.Name));
        }

        // Obter total de registros para paginação
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var tenancyDtos = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TenancyDto
            {
                Id = t.Id,
                Name = t.Name,
                Url = t.Url,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // Retornar a resposta paginada
        return new PagedResponse<List<TenancyDto>>(
            tenancyDtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}