using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; set; }
}
