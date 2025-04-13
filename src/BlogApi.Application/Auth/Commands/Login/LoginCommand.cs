using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Username { get; set; }
    public string Password { get; set; }
}
