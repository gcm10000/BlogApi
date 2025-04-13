using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserDto>
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
