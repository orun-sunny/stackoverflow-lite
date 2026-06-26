using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Data.DbContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserProfile>(entity =>
        {
            entity.HasIndex(p => p.UserId).IsUnique();
            entity.Property(p => p.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Reputation).HasDefaultValue(0);
        });

        builder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.Name).HasMaxLength(50).IsRequired();
        });

        builder.Entity<Question>(entity =>
        {
            entity.Property(q => q.Title).HasMaxLength(200).IsRequired();
            entity.Property(q => q.Description).IsRequired();
            entity.HasMany(q => q.Tags)
                  .WithMany(t => t.Questions);
        });

        builder.Entity<Answer>(entity =>
        {
            entity.Property(a => a.Content).IsRequired();
            entity.HasOne(a => a.Question)
                  .WithMany(q => q.Answers)
                  .HasForeignKey(a => a.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
