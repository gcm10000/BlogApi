using BlogApi.Application.ApiKeys.Commands.GenerateApiKey;
using BlogApi.Application.Constants;
using BlogApi.Application.Exceptions;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Infrastructure.Identity.Data;
using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Application.Infrastructure.Identity.Services;
using BlogApi.Application.Tenancies.Dtos;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Application.Tenancies.Commands.CreateTenancy;

public class CreateTenancyCommandHandler : IRequestHandler<CreateTenancyCommand, CreateTenancyDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityContext;
    private readonly CreateApiKeyService _createApiKeyService;


    private const string DefaultPassword = "MeuBlog@123456";

    public CreateTenancyCommandHandler(
        UserManager<ApplicationUser> userManager,
        BlogDbContext context,
        IdentityDbContext identityContext,
        CreateApiKeyService createApiKeyService)
    {
        _userManager = userManager;
        _context = context;
        _identityContext = identityContext;
        _createApiKeyService = createApiKeyService;
    }

    public async Task<CreateTenancyDto> Handle(CreateTenancyCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o nome da tenancy já existe
        var existingTenancy = await _context.Tenancies
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == request.Name, cancellationToken);
        
        if (existingTenancy != null)
            throw new InvalidOperationException($"Tenancy with name {request.Name} already exists.");

        var isTenancyTableEmpty = !_context.Tenancies.Any();

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
                UpdatedAt = DateTime.UtcNow,
                IsMainTenancy = isTenancyTableEmpty
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
                IsMainTenancy = isTenancyTableEmpty,
                TenancyDomainName = request.Name,
                Name = request.Name,
                Role = isTenancyTableEmpty ? RoleConstants.RootAdmin : RoleConstants.Administrator,
                UserName = request.MainAdministratorEmail,
                Email = request.MainAdministratorEmail,
                AuthorId = author.Id,
                TenancyDomainId = tenancy.Id,
                IsProtected = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
            {
                throw new BusinessRuleException("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var generateApiKeyCommandResult = await _createApiKeyService.GenerateApiKeyAsync
            (
                name: "Default ApiKey", 
                scopes: ["post:getposts", "post:getpostbyslug"],
                tenancyDomainId: tenancy.Id,
                isProtected: true,
                cancellationToken
            );
            

            // Adiciona a role
            var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.RootAdmin);
            if (!roleResult.Succeeded)
            {
                throw new BusinessRuleException("Erro ao adicionar role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
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
                MainAdministratorEmail = user.Email,
                CreatedAt = tenancy.CreatedAt,
                UpdatedAt = tenancy.UpdatedAt,
                Email = request.MainAdministratorEmail,
                Password = DefaultPassword,
                ApiKeyDefaultName = generateApiKeyCommandResult.Name,
                ApiKeyDefaultKey = generateApiKeyCommandResult.Key
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