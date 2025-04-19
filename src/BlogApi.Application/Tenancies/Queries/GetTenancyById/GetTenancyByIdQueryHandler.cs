using BlogApi.Application.Constants;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Queries.GetTenancyById;

public class GetTenancyByIdQueryHandler : IRequestHandler<GetTenancyByIdQuery, TenancyDto>
{
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityDbContext;

    public GetTenancyByIdQueryHandler(BlogDbContext context, IdentityDbContext identityDbContext)
    {
        _context = context;
        _identityDbContext = identityDbContext;
    }

    public async Task<TenancyDto> Handle(GetTenancyByIdQuery request, CancellationToken cancellationToken)
    {
        // Buscar o inquilino pelo ID
        var tenancy = await _context.Tenancies
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenancy == null)
            return null;

        var applicationUser = await _identityDbContext.Users
            .Where(x => x.TenancyDomainId == request.Id)
            .FirstOrDefaultAsync ();

        if (applicationUser == null)
            throw new BusinessRuleException("O usuário não existe");

        if (applicationUser.Role != RoleConstants.Administrator && applicationUser.Role != RoleConstants.RootAdmin)
            throw new BusinessRuleException("O usuário deve ser um Administrador.");

        // Retornar o DTO
        return new TenancyDto
        {
            Id = tenancy.Id,
            Name = tenancy.Name,
            Url = tenancy.Url,
            IsMainTenancy = tenancy.IsMainTenancy,
            MainAdministratorEmail = applicationUser.Email!,
            CreatedAt = tenancy.CreatedAt,
            UpdatedAt = tenancy.UpdatedAt
        };
    }
}