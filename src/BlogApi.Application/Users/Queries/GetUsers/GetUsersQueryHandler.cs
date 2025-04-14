using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Common;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<List<UserDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(u =>
                u.UserName.Contains(request.Search) ||
                u.Email.Contains(request.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        var userDtos = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(x => new UserDto
            {
                Id = x.AuthorId,
                Name = x.Name,
                Email = x.Email,
                Username = x.UserName,
                Role = x.Role,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<List<UserDto>>(userDtos, total, request.Page, request.Limit);
    }
}
