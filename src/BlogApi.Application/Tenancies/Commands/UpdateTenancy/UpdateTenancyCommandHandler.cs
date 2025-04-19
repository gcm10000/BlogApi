using BlogApi.Application.Constants;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace BlogApi.Application.Tenancies.Commands.UpdateTenancy;

public class UpdateTenancyCommandHandler : IRequestHandler<UpdateTenancyCommand, TenancyDto>
{
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityDbContext;

    public UpdateTenancyCommandHandler(BlogDbContext context, IdentityDbContext identityDbContext)
    {
        _context = context;
        _identityDbContext = identityDbContext;
    }

    public async Task<TenancyDto> Handle(UpdateTenancyCommand request, CancellationToken cancellationToken)
    {
        // Buscar o inquilino pelo ID
        var tenancy = await _context.Tenancies
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenancy == null)
            throw new BusinessRuleException($"Tenancy with ID {request.Id} not found.");

        var applicationUser = await _identityDbContext.Users
            .Where(x => x.TenancyDomainId == request.Id)
            .Where(x => x.Email == request.AdministratorEmail)
            .FirstOrDefaultAsync();

        if (applicationUser == null)
            throw new BusinessRuleException("O usuário não existe");

        if (applicationUser.Role != RoleConstants.Administrator)
            throw new BusinessRuleException("O usuário deve ser um Administrador.");

        // Atualizar as informações do inquilino
        tenancy.Name = request.Name;
        tenancy.Url = tenancy.Url;
        tenancy.UpdatedAt = DateTime.UtcNow;
        tenancy.MainAuthorId = applicationUser.AuthorId;

        // Salvar as alterações no banco de dados
        await _context.SaveChangesAsync(cancellationToken);

        // Retornar o DTO atualizado
        return new TenancyDto
        {
            Id = tenancy.Id,
            Name = tenancy.Name,
            MainAdministratorEmail = applicationUser.Email!,
            CreatedAt = tenancy.CreatedAt,
            UpdatedAt = tenancy.UpdatedAt
        };
    }
}