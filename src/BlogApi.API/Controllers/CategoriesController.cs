using BlogApi.Application.Posts.Commands.AddCategory;
using BlogApi.Application.Posts.Commands.DeleteCategory;
using BlogApi.Application.Posts.Queries.GetCategoriesQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Api.Controllers;

[Route("api/v1/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Listar Categorias
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(new { categories = categories.Select(c => c.Name) });
    }

    // Adicionar Categoria
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryCommand command)
    {
        var category = await _mediator.Send(command);
        return Ok(new { success = true, category = category.Name });
    }

    // Excluir Categoria
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteCategory(string name)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand { Name = name });

        if (!success)
            return NotFound();

        return Ok(new { success = true, message = "Categoria excluída com sucesso" });
    }
}
