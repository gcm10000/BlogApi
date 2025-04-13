using BlogApi.Application.Infrastructure.Data;
using MediatR;

namespace BlogApi.Application.Posts.Commands.PostCommands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
{
    private readonly BlogDbContext _db;

    public DeletePostCommandHandler(BlogDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FindAsync(request.Id);
        if (post == null)
            throw new Exception("Post not found.");

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
