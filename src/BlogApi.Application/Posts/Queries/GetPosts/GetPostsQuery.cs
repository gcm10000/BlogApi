using BlogApi.Application.Common;
using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Posts.Queries.GetPosts;

public class GetPostsQuery : IRequest<PagedResponse<List<PostDto>>>
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Status { get; set; } = "";
    public string? Search { get; set; } = "";
    public string? Category { get; set; } = "";
}
