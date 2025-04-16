using MediatR;
using BlogApi.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace BlogApi.Application.Posts.Commands.PostCommands.CreatePost;

public class CreatePostCommand : IRequest<PostDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
}
