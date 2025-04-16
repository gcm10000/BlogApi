using BlogApi.API.Attributes;
using BlogApi.Application.Categories.Commands.AddCategory;
using BlogApi.Application.Categories.Commands.DeleteCategory;
using BlogApi.Application.Categories.Queries.GetCategories;
using BlogApi.Application.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Api.Controllers;

/// <summary>
/// Gerencia categorias de postagens do blog.
/// </summary>
[TenancyApiControllerRouteV1("categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todas as categorias.
    /// </summary>
    /// <returns>Lista de nomes das categorias.</returns>
    /// <response code="200">Categorias listadas com sucesso.</response>
    [HttpGet]
    [Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(new { categories = categories.Select(c => c.Name) });
    }

    /// <summary>
    /// Adiciona uma nova categoria.
    /// </summary>
    /// <param name="command">Dados da categoria a ser adicionada.</param>
    /// <returns>Confirmação de sucesso com nome da categoria criada.</returns>
    /// <response code="200">Categoria adicionada com sucesso.</response>
    /// <response code="400">Dados inválidos ou categoria já existente.</response>
    [HttpPost]
    [Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryCommand command)
    {
        var category = await _mediator.Send(command);
        return Ok(new { success = true, category = category.Name });
    }

    /// <summary>
    /// Exclui uma categoria pelo nome.
    /// </summary>
    /// <param name="name">Nome da categoria.</param>
    /// <returns>Confirmação de exclusão.</returns>
    /// <response code="200">Categoria excluída com sucesso.</response>
    /// <response code="404">Categoria não encontrada.</response>
    [HttpDelete("{name}")]
    [Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(string name)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand { Name = name });

        if (!success)
            return NotFound();

        return Ok(new { success = true, message = "Categoria excluída com sucesso" });
    }
}
