using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Queries.GetTenancyById;

public class GetTenancyByIdQueryHandler : IRequestHandler<GetTenancyByIdQuery, TenancyDto>
{
    private readonly BlogDbContext _context;

    public GetTenancyByIdQueryHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<TenancyDto> Handle(GetTenancyByIdQuery request, CancellationToken cancellationToken)
    {
        // Buscar o inquilino pelo ID
        var tenancy = await _context.Tenancies
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenancy == null)
            return null;

        // Retornar o DTO
        return new TenancyDto
        {
            Id = tenancy.Id,
            Name = tenancy.Name,
            Url = tenancy.Url,
            CreatedAt = tenancy.CreatedAt,
            UpdatedAt = tenancy.UpdatedAt
        };
    }
}