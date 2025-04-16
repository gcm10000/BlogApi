namespace BlogApi.Application.DTOs;

public class DashboardStatsDto
{
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
    public int TotalUsers { get; set; }
    public int TotalViews { get; set; }
}