using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponseDto>
{
    public async Task<ChangePasswordResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Lógica para alterar senha
        return new ChangePasswordResponseDto
        {
            Success = true,
            Message = "Senha alterada com sucesso"
        };
    }
}