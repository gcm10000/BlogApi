using BlogApi.Application.Infrastructure.Data;
using BlogApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Commands.PostCommands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
{
    private readonly BlogDbContext _db;
    private readonly ICurrentUserService _currentUserService;

    public DeletePostCommandHandler(BlogDbContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var tenancyId = _currentUserService.GetCurrentTenancy();

        var post = await _db.Posts
            .Include(x => x.Tenancy)
            .Where(x => x.Tenancy.DeletedAt == null)
            .Where(x => x.TenancyId == tenancyId)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (post == null)
            throw new Exception("Post not found.");

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
