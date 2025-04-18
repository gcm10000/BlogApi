using BlogApi.API.Attributes;
using BlogApi.Application.Interfaces;

public class ApiKeyAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyServices apiKeyServices)
    {
        var hasBearerToken = context.Request.Headers.TryGetValue("Authorization", out var authHeader) &&
                             authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

        if (hasBearerToken)
        {
            // A requisição é de um usuário autenticado com token JWT.
            await _next(context);
            return;
        }

        // Caso contrário, verifica se é uma integração com chave de API.
        if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyValue))
        {
            // Nenhum método de autenticação foi informado → apenas segue o fluxo
            // e deixa o [Authorize] ou filtros padrão bloquearem, se necessário.
            await _next(context);
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
        var endpoint = context.GetEndpoint();
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

        // Armazena TenantId no contexto
        context.Items["TenantId"] = result.TenantId;

        await _next(context);
    }
}
