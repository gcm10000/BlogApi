using MediatR;

namespace BlogApi.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    public Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Remover usuário
        throw new NotImplementedException();
    }
}
