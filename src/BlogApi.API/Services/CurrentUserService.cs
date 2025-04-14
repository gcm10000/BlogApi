using BlogApi.Application.Interfaces;
using System.Security.Claims;

namespace BlogApi.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserIdentityId
        => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public IEnumerable<Claim> GetClaims()
    {
        return _httpContextAccessor.HttpContext!.User.Claims.ToList();
    }

    public Claim? GetRole()
    {
        var result = GetClaims().FirstOrDefault(x => x.Type == ClaimTypes.Role);
        return result;
    }

    public string? GetCurrentRoleAsString()
    {
        var result = GetRole();
        return result?.Value;
    }
}
