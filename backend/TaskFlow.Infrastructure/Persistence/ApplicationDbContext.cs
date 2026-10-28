using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.AvatarUrl)
                .HasMaxLength(2048);

            entity.Property(user => user.CreatedAtUtc)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }
}