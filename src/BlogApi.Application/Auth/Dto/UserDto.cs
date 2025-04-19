namespace BlogApi.Application.Auth.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public required int TenancyDomainId { get; set; }
    public required string TenancyDomainName { get; set; }
    public required bool IsMainTenancy { get; set; }
    public int Posts { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
