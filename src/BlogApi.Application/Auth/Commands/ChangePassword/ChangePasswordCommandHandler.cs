using BlogApi.Application.Auth.Dto;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ChangePasswordResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return new ChangePasswordResponseDto { Success = false, Message = "Usuário não encontrado" };
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return new ChangePasswordResponseDto  { Success = false, Message = "Erro ao alterar senha", Errors = result.Errors.Select(x => $"{x.Code} - {x.Description}").ToList() };
        }

        return new ChangePasswordResponseDto { Success = true, Message = "Senha alterada com sucesso" };
    }
}