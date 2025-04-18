using BlogApi.API.Attributes;
using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using BlogApi.Application.Constants;
using BlogApi.Application.Users.Commands.CreateUser;
using BlogApi.Application.Users.Commands.DeleteUser;
using BlogApi.Application.Users.Commands.UpdateUser;
using BlogApi.Application.Users.Queries.GetUserById;
using BlogApi.Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Controllers;

[ApiController]
//[Route("api/v1/users")]
[TenancyApiControllerRouteV1("users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os usuários com suporte a paginação.
    /// </summary>
    /// <param name="query">Query contendo parâmetros de paginação (página, quantidade de usuários, etc).</param>
    /// <returns>Uma lista paginada de usuários.</returns>
    /// <response code="200">Lista de usuários retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserDto>), StatusCodes.Status200OK)]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministrator)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um usuário específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário a ser recuperado.</param>
    /// <returns>O usuário correspondente ao ID fornecido.</returns>
    /// <response code="200">Usuário encontrado e retornado com sucesso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministrator)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="tenancyId"></param>
    /// <param name="command">Comando contendo os dados do novo usuário a ser criado.</param>
    /// <returns>O usuário recém-criado.</returns>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos para criar o usuário.</response>
    [HttpPost]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministrator)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromRoute] int tenancyId, [FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { tenancyId, id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um usuário existente.
    /// </summary>
    /// <param name="id">ID do usuário a ser atualizado.</param>
    /// <param name="command">Comando contendo os dados atualizados do usuário.</param>
    /// <returns>O usuário atualizado.</returns>
    /// <response code="204">Usuário atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos para atualizar o usuário.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministrator)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Exclui um usuário pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário a ser excluído.</param>
    /// <returns>Mensagem de sucesso ou erro na exclusão.</returns>
    /// <response code="204">Usuário excluído com sucesso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministrator)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _mediator.Send(new DeleteUserCommand { Id = id });

        if (!result)
            return NotFound();

        return Ok(new { success = true, message = "Usuário excluído com sucesso" });
    }
}
