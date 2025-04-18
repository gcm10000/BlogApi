namespace BlogApi.Application.Infrastructure.Identity.Models;
public class ApiKeyScope
{
    public int ApiKeyId { get; set; }
    public ApiKey ApiKey { get; set; } = default!;

    public int ApiScopeId { get; set; }
    public ApiScope ApiScope { get; set; } = default!;
}
