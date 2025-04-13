using BlogApi.Application.Auth.Dto;
using MediatR;

namespace BlogApi.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}
