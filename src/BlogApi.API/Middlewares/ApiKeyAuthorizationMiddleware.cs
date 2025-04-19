using System.Security.Claims;
using BlogApi.API.Attributes;
using BlogApi.Application.Constants;
using BlogApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

public class ApiKeyAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyServices)
    {
        var endpoint = context.GetEndpoint();

        // Ignora rotas públicas (sem [Authorize])
        var hasAuthorize = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;
        if (!hasAuthorize)
        {
            await _next(context);
            return;
        }

        var hasBearerToken = context.Request.Headers.TryGetValue("Authorization", out var authHeader)
                             && authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

        if (hasBearerToken)
        {
            await _next(context); // O JWT será validado no pipeline de autenticação
            return;
        }

        // Tentativa com X-API-KEY
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyValue))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization required.");
            return;
        }

        var result = await apiKeyServices.GetApiKeyAsync(apiKeyValue!);

        if (!result.IsValid)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(result.ErrorMessage ?? "Unauthorized");
            return;
        }

        // Verificar escopos exigidos pela rota
        var requiredScopes = endpoint?.Metadata.GetOrderedMetadata<RequireApiScopeAttribute>();

        if (requiredScopes?.Any() == true)
        {
            var hasAll = requiredScopes.All(rs => result.Scopes.Contains(rs.Scope));
            if (!hasAll)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Missing required scope(s)");
                return;
            }
        }

        // Cria identidade fake com claims da API Key
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, $"apikey:{result.ApiKeyId}"),
            new Claim(CustomClaimTypes.Name, result.Name),
            new Claim(CustomClaimTypes.TenancyDomainId, result.TenancyDomainId.ToString()),
            new Claim(ClaimTypes.Role, RoleConstants.Administrator)
        };

        foreach (var scope in result.Scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "ApiKey"));
        await _next(context);
    }
}
