using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Retornar usuário por ID
        throw new NotImplementedException();
    }
}
