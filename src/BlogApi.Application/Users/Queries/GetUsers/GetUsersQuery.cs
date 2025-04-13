using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using MediatR;

namespace BlogApi.Application.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PagedResponse<UserDto>>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public string? Search { get; set; }
}
