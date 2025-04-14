using BlogApi.Application.Auth.Dto;

namespace BlogApi.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
}