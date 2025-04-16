using MediatR;
using BlogApi.Application.DTOs;

namespace BlogApi.Application.Posts.Queries.GetPostById;

public class GetPostBySlugQuery : IRequest<PostDto?>
{
    public string Slug { get; set; }
    public int TenancyId { get; set; }

    public GetPostBySlugQuery(string slug, int tenancyId)
    {
        Slug = slug;
        TenancyId = tenancyId;
    }
}
