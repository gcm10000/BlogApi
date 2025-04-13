using MediatR;

namespace BlogApi.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}
