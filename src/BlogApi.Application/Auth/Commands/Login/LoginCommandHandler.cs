using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Interfaces;
using MediatR;

namespace BlogApi.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(new LoginDto
        {
            Username = request.Username,
            Password = request.Password
        });
    }
}