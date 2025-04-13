using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UserDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
