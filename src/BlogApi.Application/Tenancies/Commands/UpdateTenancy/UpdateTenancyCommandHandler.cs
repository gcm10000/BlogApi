using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Tenancies.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace BlogApi.Application.Tenancies.Commands.UpdateTenancy;

public class UpdateTenancyCommandHandler : IRequestHandler<UpdateTenancyCommand, TenancyDto>
{
    private readonly BlogDbContext _context;

    public UpdateTenancyCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<TenancyDto> Handle(UpdateTenancyCommand request, CancellationToken cancellationToken)
    {
        // Buscar o inquilino pelo ID
        var tenancy = await _context.Tenancies
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenancy == null)
            throw new KeyNotFoundException($"Tenancy with ID {request.Id} not found.");

        // Atualizar as informações do inquilino
        tenancy.Name = request.Name;
        tenancy.Url = tenancy.Url;
        tenancy.UpdatedAt = DateTime.UtcNow;

        // Salvar as alterações no banco de dados
        await _context.SaveChangesAsync(cancellationToken);

        // Retornar o DTO atualizado
        return new TenancyDto
        {
            Id = tenancy.Id,
            Name = tenancy.Name,
            CreatedAt = tenancy.CreatedAt,
            UpdatedAt = tenancy.UpdatedAt
        };
    }
}