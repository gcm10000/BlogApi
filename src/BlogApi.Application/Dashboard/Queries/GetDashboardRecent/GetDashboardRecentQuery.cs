using MediatR;

namespace BlogApi.Application.Dashboard.Queries.GetDashboardRecent;

public class GetDashboardRecentQuery : IRequest<DashboardRecentDto> { }

public class DashboardRecentDto
{
    public List<RecentPostDto> Posts { get; set; }
    public List<RecentUserDto> Users { get; set; }
}

public class RecentPostDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecentUserDto
{
    public int Id { get; set; }
    //public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
