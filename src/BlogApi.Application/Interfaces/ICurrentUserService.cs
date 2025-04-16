using System.Security.Claims;

namespace BlogApi.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserIdentityId { get; }

    IEnumerable<Claim> GetClaims();
    Claim? GetRole();
    string? GetCurrentRoleAsString();
    int GetCurrentTenancy();
    int GetCurrentAuthorId();
}
