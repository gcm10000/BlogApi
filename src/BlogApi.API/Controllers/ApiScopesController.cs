using BlogApi.API.Attributes;
using BlogApi.API.Helpers;
using BlogApi.Application.ApiScopes;
using BlogApi.Application.Constants;
using BlogApi.Application.Infrastructure.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BlogApi.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ApiScopesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public ApiScopesController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpGet]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    public async Task<IActionResult> GetAll()
    {
        var ENVIRONMENT_VARIABLE_BACK_END_URL = _configuration["ENVIRONMENT_VARIABLE_BACK_END_URL"]!.ToString();

        var scopesFromDb = await _mediator.Send(new GetAllApiScopesQuery());

        var scopesWithRoutes = ApiScopeReflectionHelper.ExtractApiScopes(ENVIRONMENT_VARIABLE_BACK_END_URL); // método que vamos criar abaixo

        // Match e atualizar o Name com o padrão: escopo | VERB /rota
        var updated = scopesFromDb.Select(scope =>
        {
            var match = scopesWithRoutes.FirstOrDefault(r => r.Name.StartsWith(scope.Name, StringComparison.OrdinalIgnoreCase));
            var apiScopeDto = new ApiScopeDto() 
            { 
                Id = scope.Id
            };

            if (match != null)
            {
                apiScopeDto.Name = match.Name;
                apiScopeDto.Endpoint = match.Endpoint;
                apiScopeDto.Verb = match.Verb;
            }

            return apiScopeDto;
        }).ToList();

        return Ok(updated);
    }

}