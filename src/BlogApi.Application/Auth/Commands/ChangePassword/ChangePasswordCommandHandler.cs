using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Infrastructure.Identity.Models;
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

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            return new ChangePasswordResponseDto  { Success = false, Message = "Erro ao alterar senha", Errors = result.Errors.Select(x => $"{x.Code} - {x.Description}").ToList() };
        }

        user.PasswordChangeRequired = false;
        await _userManager.UpdateAsync(user);

        return new ChangePasswordResponseDto { Success = true, Message = "Senha alterada com sucesso" };
    }
}