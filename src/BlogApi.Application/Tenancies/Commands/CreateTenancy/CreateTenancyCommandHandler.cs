using BlogApi.Application.Constants;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Tenancies.Dtos;
using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Identity.Data;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Tenancies.Commands.CreateTenancy;
/*
 
 
 {
  "name": "Serralheria GJM",
  "email": "serralheriagjm@gmail.com",
  "url": "http://www.serralheriagjm.com.br/"
}
 
 */
public class CreateTenancyCommandHandler : IRequestHandler<CreateTenancyCommand, TenancyDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityContext;

    private const string DefaultPassword = "MeuBlog@123456";

    public CreateTenancyCommandHandler(UserManager<ApplicationUser> userManager, BlogDbContext context, IdentityDbContext identityContext)
    {
        _userManager = userManager;
        _context = context;
        _identityContext = identityContext;
    }

    public async Task<TenancyDto> Handle(CreateTenancyCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o nome da tenancy já existe
        var existingTenancy = await _context.Tenancies
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == request.Name, cancellationToken);
        
        if (existingTenancy != null)
            throw new InvalidOperationException($"Tenancy with name {request.Name} already exists.");


        await using var transactionBlogDb = await _context.Database.BeginTransactionAsync(cancellationToken);
        await using var transactionIdentityDb = await _identityContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Criar a nova instância de Tenancy
            var tenancy = new Tenancy
            {
                Name = request.Name,
                Url = request.Url,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Adicionar a tenancy ao contexto e salvar
            _context.Tenancies.Add(tenancy);
            await _context.SaveChangesAsync(cancellationToken);


            // Cria o Author
            var author = new Author
            {
                Name = request.Name,
                Bio = "",
                TenancyId = tenancy.Id
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);

            // Cria o ApplicationUser
            var user = new ApplicationUser
            {
                PasswordChangeRequired = true,
                Name = request.Name,
                Role = RoleConstants.Administrator,
                UserName = request.Email,
                Email = request.Email,
                AuthorId = author.Id,
                TenancyDomainId = tenancy.Id,
                IsProtected = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Adiciona a role
            var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.Administrator);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Erro ao adicionar role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Commit em ambas as transações
            await transactionBlogDb.CommitAsync(cancellationToken);
            await transactionIdentityDb.CommitAsync(cancellationToken);


            // Retornar o DTO da tenancy criada
            return new CreateTenancyDto
            {
                Id = tenancy.Id,
                Url = tenancy.Url,
                Name = tenancy.Name,
                CreatedAt = tenancy.CreatedAt,
                UpdatedAt = tenancy.UpdatedAt,
                Email = request.Email,
                Password = DefaultPassword
            };
        }
        catch
        {
            await transactionBlogDb.RollbackAsync(cancellationToken);
            await transactionIdentityDb.RollbackAsync(cancellationToken);
            throw;
        }

    }
}