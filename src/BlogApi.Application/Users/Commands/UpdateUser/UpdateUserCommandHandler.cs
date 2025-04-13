using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    public Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Atualizar usuário
        throw new NotImplementedException();
    }
}
