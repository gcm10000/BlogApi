using BlogApi.Application.DTOs;
using MediatR;

namespace BlogApi.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto> { }
