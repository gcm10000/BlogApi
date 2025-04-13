using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<ChangePasswordResponseDto>
{
    public string Username { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
