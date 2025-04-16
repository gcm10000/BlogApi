using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using MediatR;

namespace BlogApi.Application.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PagedResponse<List<UserDto>>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
}
