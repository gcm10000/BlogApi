using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Interfaces;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _tokenService.GetUserByRefreshTokenAsync(request.RefreshToken);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Access_Token = null,
                Refresh_Token = null,
                User = null,
                Success = false,
                Message = "Refresh token inválido ou expirado."
            };
        }

        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        var userDto = new UserDto
        {
            Id = user.AuthorId,
            Username = user.UserName,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return new AuthResponseDto
        {
            Access_Token = newAccessToken,
            Refresh_Token = newRefreshToken,
            User = userDto,
            Success = true
        };
    }
}