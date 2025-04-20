using MediatR;
using BlogApi.Application.DTOs;

namespace BlogApi.Application.Posts.Queries.GetPostById;

public class GetPostByIdQuery : IRequest<PostDto?>
{
    public int Id { get; set; }

    public GetPostByIdQuery(int id)
    {
        Id = id;
    }
}
