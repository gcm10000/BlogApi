namespace BlogApi.Application.Infrastructure.Identity.Models;
public class ApiScope
{
    public int Id { get; set; }
    public string Name { get; set; } = default!; // Ex: read:posts, admin:users
}
