using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Identity.Data;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly BlogDbContext _context;
    private readonly IdentityDbContext _identityContext;

    public CreateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        BlogDbContext context,
        IdentityDbContext identityContext)
    {
        _userManager = userManager;
        _context = context;
        _identityContext = identityContext;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await using var transactionBlogDb = await _context.Database.BeginTransactionAsync(cancellationToken);
        await using var transactionIdentityDb = await _identityContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Cria o Author
            var author = new Author
            {
                Name = request.Name,
                Bio = ""
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);

            // Cria o ApplicationUser
            var user = new ApplicationUser
            {
                Name = request.Name,
                Role = request.Role,
                UserName = request.Email,
                Email = request.Email,
                AuthorId = author.Id
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Adiciona a role
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                throw new Exception("Erro ao adicionar role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Commit em ambas as transações
            await transactionBlogDb.CommitAsync(cancellationToken);
            await transactionIdentityDb.CommitAsync(cancellationToken);

            return new UserDto
            {
                Id = user.AuthorId,
                Username = user.UserName,
                Email = user.Email,
                Name = author.Name,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
