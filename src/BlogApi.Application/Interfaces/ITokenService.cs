using BlogApi.Application.Infrastructure.Identity.Models;

namespace BlogApi.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
}