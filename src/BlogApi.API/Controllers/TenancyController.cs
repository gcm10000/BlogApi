using BlogApi.Application.Common;
using BlogApi.Application.Constants;
using BlogApi.Application.Tenancies.Commands.CreateTenancy;
using BlogApi.Application.Tenancies.Commands.DeleteTenancy;
using BlogApi.Application.Tenancies.Commands.UpdateTenancy;
using BlogApi.Application.Tenancies.Dtos;
using BlogApi.Application.Tenancies.Queries.GetTenancies;
using BlogApi.Application.Tenancies.Queries.GetTenancyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Controllers;

[Route("api/v1/tenancy")]
[ApiController]
public class TenancyController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenancyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Criar novo inquilino
    // Permite que um administrador crie um novo inquilino.
    // Produz uma resposta 201 (Created) com os detalhes do inquilino recém-criado.
    // Responde com 400 (Bad Request) caso os dados do inquilino sejam inválidos.
    [HttpPost]
    [Authorize(Roles = RoleConstants.RootAdmin)]
    [ProducesResponseType(typeof(TenancyDto), StatusCodes.Status201Created)] // Retorna TenancyDto com 201 Created
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Retorna 400 se os dados forem inválidos
    public async Task<IActionResult> CreateTenancy([FromBody] CreateTenancyCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTenancyById), new { id = result.Id }, result);
    }

    // Obter todos os inquilinos
    // Permite que um administrador recupere a lista de todos os inquilinos.
    // Produz uma resposta 200 (OK) com a lista paginada de inquilinos.
    [HttpGet]
    [Authorize(Roles = RoleConstants.RootAdmin)]
    [ProducesResponseType(typeof(PagedResponse<TenancyDto>), StatusCodes.Status200OK)] // Retorna a lista paginada de TenancyDto
    public async Task<IActionResult> GetTenancies()
    {
        var result = await _mediator.Send(new GetTenanciesQuery());
        return Ok(result);
    }

    // Obter inquilino por ID
    // Permite que um administrador recupere os detalhes de um inquilino específico pelo seu ID.
    // Produz uma resposta 200 (OK) com o inquilino encontrado.
    // Responde com 404 (Not Found) se o inquilino não for encontrado.
    [HttpGet("{id}")]
    [Authorize(Roles = RoleConstants.RootAdmin)]
    [ProducesResponseType(typeof(TenancyDto), StatusCodes.Status200OK)] // Retorna TenancyDto com 200 OK
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Retorna 404 se o inquilino não for encontrado
    public async Task<IActionResult> GetTenancyById(int id)
    {
        var result = await _mediator.Send(new GetTenancyByIdQuery { Id = id });
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    // Atualizar inquilino
    // Permite que um administrador atualize os detalhes de um inquilino existente.
    // Produz uma resposta 200 (OK) com os detalhes atualizados do inquilino.
    // Responde com 400 (Bad Request) se os dados forem inválidos.
    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.RootAdmin)]
    [ProducesResponseType(typeof(TenancyDto), StatusCodes.Status200OK)] // Retorna TenancyDto com 200 OK
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Retorna 400 se os dados forem inválidos
    public async Task<IActionResult> UpdateTenancy(int id, [FromBody] UpdateTenancyCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Excluir inquilino
    // Permite que um administrador exclua um inquilino existente.
    // Produz uma resposta 200 (OK) com uma mensagem de sucesso.
    // Responde com 404 (Not Found) se o inquilino não for encontrado.
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleConstants.RootAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)] // Retorna 200 OK com a mensagem de sucesso
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Retorna 404 se o inquilino não for encontrado
    public async Task<IActionResult> DeleteTenancy(int id)
    {
        var result = await _mediator.Send(new DeleteTenancyCommand { Id = id });
        if (!result)
            return NotFound();
        return Ok(new { success = result, message = "Inquilino excluído com sucesso" });
    }
}
