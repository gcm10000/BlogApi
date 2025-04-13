using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using MediatR;

namespace BlogApi.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
{
    public Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Buscar e retornar lista paginada de usuários
        throw new NotImplementedException();
    }
}
