using MediatR;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Application.Posts.Commands.CreatePost;
using BlogApi.Application.Posts.Queries.GetPostById;
using BlogApi.Application.Posts.Queries.GetPostsQuery;
using BlogApi.Application.Posts.Commands.UpdatePost;
using BlogApi.Application.Posts.Commands.DeletePost;
using BlogApi.Application.Posts.Commands.UpdatePostStatus;

namespace BlogApi.API.Controllers;

[Route("api/v1/posts")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Listar Posts
    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] GetPostsQuery query)
    {
        var posts = await _mediator.Send(query);
        return Ok(posts);
    }

    // Obter Post por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _mediator.Send(new GetPostByIdQuery(id));
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    // Criar Post
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {
        var post = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    // Atualizar Post
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostCommand command)
    {
        command.Id = id;
        var updatedPost = await _mediator.Send(command);

        if (updatedPost == null)
            return NotFound();

        return Ok(updatedPost);
    }

    // Excluir Post
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var success = await _mediator.Send(new DeletePostCommand { Id = id });

        if (!success)
            return NotFound();

        return Ok(new { success = true, message = "Post excluído com sucesso" });
    }

    // Alterar Status do Post
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePostStatus(int id, [FromBody] UpdatePostStatusCommand command)
    {
        command.Id = id;
        var post = await _mediator.Send(command);

        if (post == null)
            return NotFound();

        return Ok(post);
    }
}
