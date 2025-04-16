using Microsoft.AspNetCore.Mvc;
using MediatR;
using BlogApi.Application.Dashboard.Queries.GetDashboardRecent;
using BlogApi.Application.Dashboard.Queries.GetDashboardStats;
using Microsoft.AspNetCore.Authorization;
using BlogApi.Application.Constants;
using BlogApi.API.Attributes;

namespace BlogApi.API.Controllers;

[ApiController]
[TenancyApiControllerRouteV1("dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    [Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(result);
    }

    [HttpGet("recent")]
    [Authorize(Roles = RoleConstants.AdministratorAndAuthor)]
    public async Task<IActionResult> GetRecent()
    {
        var result = await _mediator.Send(new GetDashboardRecentQuery());
        return Ok(result);
    }
}
