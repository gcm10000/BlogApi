using BlogApi.API.Attributes;
using BlogApi.Application.ApiScopes;
using BlogApi.Application.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ApiScopesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiScopesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllApiScopesQuery());
        return Ok(result);
    }
}