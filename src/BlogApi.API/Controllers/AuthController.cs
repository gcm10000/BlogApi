using BlogApi.API.Attributes;
using BlogApi.API.Dto;
using BlogApi.Application.Auth.Commands.ChangePassword;
using BlogApi.Application.Auth.Commands.Login;
using BlogApi.Application.Auth.Commands.RefreshToken;
using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Constants;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
//[TenancyApiControllerRouteV1("auth")]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Realiza o login do usuário, retornando um token de autenticação.
    /// </summary>
    /// <param name="command">Comando contendo as credenciais de login (usuário e senha).</param>
    /// <returns>Um token de autenticação, se as credenciais forem válidas.</returns>
    /// <response code="200">Login realizado com sucesso, token gerado.</response>
    /// <response code="401">Credenciais inválidas.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (result == null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        return Ok(result);
    }

    /// <summary>
    /// Realiza a renovação do token de autenticação utilizando o refresh token.
    /// </summary>
    /// <param name="command">Comando contendo o refresh token.</param>
    /// <returns>Um novo token de autenticação.</returns>
    /// <response code="200">Refresh token válido, novo token gerado.</response>
    /// <response code="400">Refresh token inválido ou expirado.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Altera a senha de um usuário autenticado.
    /// </summary>
    /// <param name="command">Comando contendo a senha atual e a nova senha.</param>
    /// <returns>Uma resposta indicando se a alteração foi bem-sucedida.</returns>
    /// <response code="200">Senha alterada com sucesso.</response>
    /// <response code="401">Usuário não autorizado a alterar a senha.</response>
    [HttpPost("change-password")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleConstants.RootAdminAndAdministratorAndAuthor)]
    public IActionResult Me()
    {
        var claims = _currentUserService.GetClaims();
        var passwordChangeRequired = bool.Parse(claims.FirstOrDefault(x => x.Type == CustomClaimTypes.PasswordChangeRequired)?.Value!);
        var email = claims.FirstOrDefault(x => x.Type == CustomClaimTypes.EmailAddress)?.Value;
        var name = claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Name)?.Value;
        var tenancyDomainId = claims.FirstOrDefault(x => x.Type == CustomClaimTypes.TenancyDomainId)?.Value;
        var tenancyDomainName = claims.FirstOrDefault(x => x.Type == CustomClaimTypes.TenancyDomainName)?.Value;
        var isMainTenancy = claims.FirstOrDefault(x => x.Type == CustomClaimTypes.IsMainTenancy)?.Value;

        var role = _currentUserService.GetCurrentRoleAsString();
        return Ok(new AuthResponse
        {
            Username = email,
            TenancyId = tenancyDomainId,
            IsMainTenancy = bool.Parse(isMainTenancy),
            TenancyName = tenancyDomainName,
            Name = name,
            Role = role,
            PasswordChangeRequired = passwordChangeRequired
        });
    }
}
