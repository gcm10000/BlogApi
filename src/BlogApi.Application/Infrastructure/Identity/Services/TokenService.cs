using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Application.Infrastructure.Identity.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        // Lógica para gerar o JWT com claims e validade
        return Task.FromResult("new_access_token");
    }

    public Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        // Lógica para gerar e salvar um refresh token
        return Task.FromResult("new_refresh_token");
    }

    public Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        // Lógica para recuperar o usuário com base no refresh token armazenado
        return Task.FromResult<ApplicationUser?>(null);
    }
}