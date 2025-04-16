using BlogApi.Application.Auth.Dto;
using BlogApi.Application.Interfaces;
using BlogApi.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var currentTenancy = _currentUserService.GetCurrentTenancy();

        var user = await _userManager.Users
            .Where(x => x.TenancyDomainId == currentTenancy)
            .FirstOrDefaultAsync(u => u.AuthorId == request.Id, cancellationToken);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.AuthorId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}