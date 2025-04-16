using BlogApi.Application.Infrastructure.Data;
using BlogApi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.RegisterPostView;

public class RegisterPostViewCommandHandler : INotificationHandler<RegisterPostViewCommand>
{
    private readonly BlogDbContext _context;

    public RegisterPostViewCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RegisterPostViewCommand request, CancellationToken cancellationToken)
    {
        var timeLimit = DateTime.UtcNow.AddMinutes(-10);

        var alreadyViewed = await _context.PostViews.AnyAsync(v =>
            v.PostId == request.PostId &&
            v.IPAddress == request.IPAddress &&
            v.UserAgent == request.UserAgent &&
            v.ViewedAt >= timeLimit,
            cancellationToken
        );

        if (!alreadyViewed)
        {
            var postView = new PostView
            {
                PostId = request.PostId,
                IPAddress = request.IPAddress,
                UserAgent = request.UserAgent,
                ViewedAt = DateTime.UtcNow,
            };

            await _context.PostViews.AddAsync(postView);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
