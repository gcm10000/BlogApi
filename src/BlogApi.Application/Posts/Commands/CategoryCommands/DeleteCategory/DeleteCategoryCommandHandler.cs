using BlogApi.Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Application.Posts.Commands.CategoryCommands.DeleteCategory;

// Command Handler
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly BlogDbContext _context;

    public DeleteCategoryCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
