using Microsoft.AspNetCore.Identity;

namespace BlogApi.Application.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int AuthorId { get; set; }
    public int TenancyDomainId { get; set; }
    public required string TenancyDomainName { get; set; }
    public bool IsMainTenancy { get; set; }
    public bool PasswordChangeRequired { get; set; }

    public bool IsProtected { get; set; } = false;
}