using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Lógica de autenticação
        return new AuthResponseDto();
    }
}