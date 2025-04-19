using BlogApi.API.Attributes;
using BlogApi.Application.ApiKeys.Commands;
using BlogApi.Application.ApiKeys.Commands.GenerateApiKey;
using BlogApi.Application.ApiKeys.Commands.RevokeApiKey;
using BlogApi.Application.ApiKeys.Dto;
using BlogApi.Application.ApiKeys.Queries.GetApiKey;
using BlogApi.Application.ApiKeys.Queries.ListApiKeys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Controllers;

[ApiController]
[TenancyApiControllerRouteV1("apikeys")]
public class ApiKeysController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiKeysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gera uma nova API Key para o domínio informado.
    /// </summary>
    /// <param name="command">Dados do domínio e escopos</param>
    /// <returns>API Key gerada</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateApiKeyResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> Generate([FromBody] GenerateApiKeyCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Generate), result);
    }

    /// <summary>
    /// Revoga uma API Key existente.
    /// </summary>
    /// <param name="id">Chave a ser revogada</param>
    /// <returns>True se revogada com sucesso</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        var result = await _mediator.Send(new RemoveApiKeyCommand(id));
        if (!result)
            return NotFound();

        return Ok(result);
    }

    /// </summary>
    /// <summary>
    /// Obtém um apiKey específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do apiKey a ser recuperado.</param>
    /// <returns>O apiKey correspondente ao ID fornecido.</returns>
    /// <response code="200">ApiKey encontrado e retornado com sucesso.</response>
    /// <response code="404">ApiKey não encontrado.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiKeyDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _mediator.Send(new GetApiKeyQuery(id));
        if (result is  null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Lista todas as API Keys de um domínio (Tenancy).
    /// </summary>
    /// <returns>Lista de chaves</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ApiKeyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? name)
    {
        var result = await _mediator.Send(new ListApiKeysQuery(name));
        return Ok(result);
    }

}
