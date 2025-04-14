using Microsoft.EntityFrameworkCore;
using BlogApi.Domain.Entities;

namespace BlogApi.Application.Infrastructure.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostCategory>()
            .HasKey(pc => new { pc.PostId, pc.CategoryId });

        modelBuilder.Entity<PostCategory>()
            .HasOne(pc => pc.Post)
            .WithMany(p => p.PostCategories)
            .HasForeignKey(pc => pc.PostId);

        modelBuilder.Entity<PostCategory>()
            .HasOne(pc => pc.Category)
            .WithMany(c => c.PostCategories)
            .HasForeignKey(pc => pc.CategoryId);
    }
}
