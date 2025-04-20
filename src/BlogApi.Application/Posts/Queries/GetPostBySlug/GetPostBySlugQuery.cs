using MediatR;
using BlogApi.Application.DTOs;

namespace BlogApi.Application.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQuery : IRequest<PostDto?>
{
    public string Slug { get; set; }

    public GetPostBySlugQuery(string slug)
    {
        Slug = slug;
    }
}
