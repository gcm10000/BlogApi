using BlogApi.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BlogApi.Application.Posts.Commands.PostCommands.UpdatePost;

public class UpdatePostCommand : IRequest<PostDto>
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
}
