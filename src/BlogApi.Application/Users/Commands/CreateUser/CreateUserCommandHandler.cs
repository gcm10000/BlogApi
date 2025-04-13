using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Criar novo usuário
        throw new NotImplementedException();
    }
}
