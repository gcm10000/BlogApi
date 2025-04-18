using BlogApi.Application.Infrastructure.Identity.Models;
using BlogApi.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Infrastructure.Identity.Data;

public class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<ApiScope> ApiScopes { get; set; }
    public DbSet<ApiKeyScope> ApiKeyScopes { get; set; }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKeyScope>()
            .HasKey(x => new { x.ApiKeyId, x.ApiScopeId });

        modelBuilder.Entity<ApiKeyScope>()
            .HasOne(x => x.ApiKey)
            .WithMany(k => k.ApiKeyScopes)
            .HasForeignKey(x => x.ApiKeyId);

        modelBuilder.Entity<ApiKeyScope>()
            .HasOne(x => x.ApiScope)
            .WithMany()
            .HasForeignKey(x => x.ApiScopeId);

        base.OnModelCreating(modelBuilder);
    }
}