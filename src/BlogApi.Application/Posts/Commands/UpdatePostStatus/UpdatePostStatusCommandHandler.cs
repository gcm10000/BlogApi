using BlogApi.Application.DTOs;
using BlogApi.Application.Infrastructure.Data;
using MediatR;

namespace BlogApi.Application.Posts.Commands.UpdatePostStatus;

public class UpdatePostStatusCommandHandler : IRequestHandler<UpdatePostStatusCommand, PostDto>
{
    private readonly BlogDbContext _db;

    public UpdatePostStatusCommandHandler(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<PostDto> Handle(UpdatePostStatusCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FindAsync(request.Id);
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
