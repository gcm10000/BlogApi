using Microsoft.EntityFrameworkCore;
using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlogApi.Application.Infrastructure.Data;

public class BlogDbContext : DbContext
{
    private readonly IEnumerable<ISaveChangesInterceptor> _auditableEntitySaveChangesInterceptor;

    public BlogDbContext(DbContextOptions<BlogDbContext> options, IEnumerable<ISaveChangesInterceptor> auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Tenancy> Tenancies { get; set; }
    public DbSet<PostView> PostViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasIndex(p => new { p.TenancyId, p.Slug })
            .IsUnique();


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
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

    }
}
