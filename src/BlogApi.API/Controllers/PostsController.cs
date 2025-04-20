using MediatR;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Application.Posts.Queries.GetPostById;
using BlogApi.Application.Posts.Commands.PostCommands.CreatePost;
using BlogApi.Application.Posts.Commands.PostCommands.UpdatePost;
using BlogApi.Application.Posts.Commands.PostCommands.UpdatePostStatus;
using BlogApi.Application.Posts.Commands.PostCommands.DeletePost;
using BlogApi.Application.Posts.Queries.GetPosts;
using Microsoft.AspNetCore.Authorization;
using BlogApi.Application.Constants;
using BlogApi.Application.Common;
using BlogApi.Application.DTOs;
using BlogApi.Application.RegisterPostView;
using BlogApi.API.Attributes;
using BlogApi.Application.Posts.Queries.GetPostBySlug;

namespace BlogApi.API.Controllers;

//[Route("api/v1/posts")]
[TenancyApiControllerRouteV1("posts")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPublisher _publisher;

    public PostsController(IMediator mediator, IPublisher publisher)
    {
        _mediator = mediator;
        _publisher = publisher;
    }

    /// <summary>
    /// Lista todos os posts, com suporte a paginação.
    /// </summary>
    /// <param name="query">Query contendo parâmetros de paginação (página, quantidade de posts, etc).</param>
    /// <returns>Uma lista paginada de posts.</returns>
    /// <response code="200">Lista de posts retornada com sucesso.</response>
    [HttpGet]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [RequireApiScope("post:getposts")]
    [ProducesResponseType(typeof(PagedResponse<PostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts([FromQuery] GetPostsQuery query)
    {
        var posts = await _mediator.Send(query);
        return Ok(posts);
    }

    /// <summary>
    /// Obtém um post específico pelo seu ID.
    /// </summary>
    /// <param name="tenancyId">TenancyId do post a ser recuperado.</param>
    /// <param name="id">ID do post a ser recuperado.</param>
    /// <returns>O post correspondente ao ID fornecido.</returns>
    /// <response code="200">Post encontrado e retornado com sucesso.</response>
    /// <response code="404">Post não encontrado.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireApiScope("post:getpostbyid")]
    public async Task<IActionResult> GetPostById([FromRoute] int tenancyId, [FromRoute] int id)
    {
        var post = await _mediator.Send(new GetPostByIdQuery(id, tenancyId));
        if (post == null)
            return NotFound();

        return Ok(post);
    }

    /// <summary>
    /// Obtém um post específico pelo seu Slug dentro de um contexto de tenant (inquilino/cliente).
    /// </summary>
    /// <param name="slug">
    /// Slug único do post, utilizado como identificador amigável na URL. Geralmente gerado a partir do título do post.
    /// </param>
    /// <returns>O post correspondente ao Slug e Tenant fornecidos.</returns>
    /// <response code="200">Post encontrado e retornado com sucesso.</response>
    /// <response code="404">Post não encontrado.</response>
    [HttpGet("slug/{slug}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireApiScope("post:getpostbyslug")]
    public async Task<IActionResult> GetPostBySlug([FromRoute] string slug)
    {
        var post = await _mediator.Send(new GetPostBySlugQuery(slug));
        if (post == null)
            return NotFound();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        await _publisher.Publish(new RegisterPostViewCommand(post.Id, ip, userAgent));

        return Ok(post);
    }

    ///// <summary>
    ///// Cria um novo post.
    ///// </summary>
    ///// <param name="command">Comando contendo os dados do novo post a ser criado.</param>
    ///// <returns>O post recém-criado.</returns>
    ///// <response code="201">Post criado com sucesso.</response>
    ///// <response code="400">Dados inválidos fornecidos para criar o post.</response>
    //[HttpPost]
    //[Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    //[ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    //{
    //    var post = await _mediator.Send(command);
    //    return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    //}

    ///// <summary>
    ///// Atualiza um post existente.
    ///// </summary>
    ///// <param name="id">ID do post a ser atualizado.</param>
    ///// <param name="command">Comando contendo os dados atualizados do post.</param>
    ///// <returns>O post atualizado.</returns>
    ///// <response code="204">Post atualizado com sucesso.</response>
    ///// <response code="400">Dados inválidos fornecidos para atualizar o post.</response>
    ///// <response code="404">Post não encontrado.</response>
    //[HttpPut("{id}")]
    //[Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostCommand command)
    //{
    //    command.Id = id;
    //    var updatedPost = await _mediator.Send(command);

    //    if (updatedPost == null)
    //        return NotFound();

    //    return Ok(updatedPost);
    //}


    /// <summary>
    /// Cria um novo post.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    [RequireApiScope("post:createpost")]
    public async Task<IActionResult> CreatePost([FromRoute] int tenancyId, [FromForm] CreatePostCommand command)
    {
        var post = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPostById), new { tenancyId, id = post.Id }, post);
    }

    /// <summary>
    /// Atualiza um post existente.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes("multipart/form-data")]
    [RequireApiScope("post:updatepost")]
    public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostCommand command)
    {
        command.Id = id;
        var updatedPost = await _mediator.Send(command);

        if (updatedPost == null)
            return NotFound();

        return Ok(updatedPost);
    }


    /// <summary>
    /// Exclui um post pelo seu ID.
    /// </summary>
    /// <param name="id">ID do post a ser excluído.</param>
    /// <returns>Mensagem de sucesso ou erro na exclusão.</returns>
    /// <response code="204">Post excluído com sucesso.</response>
    /// <response code="404">Post não encontrado.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireApiScope("post:deletepost")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var success = await _mediator.Send(new DeletePostCommand { Id = id });

        if (!success)
            return NotFound();

        return Ok(new { success = true, message = "Post excluído com sucesso" });
    }

    /// <summary>
    /// Atualiza o status de um post (ex: ativo ou inativo).
    /// </summary>
    /// <param name="id">ID do post cujo status será alterado.</param>
    /// <param name="command">Comando contendo o novo status do post.</param>
    /// <returns>O post com o status atualizado.</returns>
    /// <response code="200">Status do post atualizado com sucesso.</response>
    /// <response code="404">Post não encontrado.</response>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireApiScope("post:updatepoststatus")]
    public async Task<IActionResult> UpdatePostStatus(int id, [FromBody] UpdatePostStatusCommand command)
    {
        command.Id = id;
        var post = await _mediator.Send(command);

        if (post == null)
            return NotFound();

        return Ok(post);
    }
}
