using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Commands.PostCommands.UpdatePostStatus;

public class UpdatePostStatusCommandHandler : IRequestHandler<UpdatePostStatusCommand, PostDto>
{
    private readonly BlogDbContext _db;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePostStatusCommandHandler(BlogDbContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUserService = currentUserService;
    }

    public async Task<PostDto> Handle(UpdatePostStatusCommand request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancyDomainId();

        var post = await _db.Posts
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .FirstOrDefaultAsync(x => x.Id == request.Id);
        if (post == null)
            throw new Exception("Post not found.");

        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new PostDto
        {
            Id = post.Id,
            Status = post.Status,
            UpdatedAt = post.UpdatedAt
        };
    }
}
