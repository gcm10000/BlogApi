namespace BlogApi.Application.Infrastructure.Identity.Dtos;
public class ApiScopeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Verb { get; set; } = default!;
    public string Endpoint { get; set; }
}
