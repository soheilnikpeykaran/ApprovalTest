using Approval.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Approval.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Request> Requests => Set<Request>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(e =>
        {
            e.ToTable("Users"); e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(320).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
        });
        builder.Entity<Role>(e =>
        {
            e.ToTable("Roles"); e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });
        builder.Entity<UserRole>(e =>
        {
            e.ToTable("UserRoles"); e.HasKey(x => new { x.UserId, x.RoleId });
            e.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
        });
        builder.Entity<Request>(e =>
        {
            e.ToTable("Requests"); e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18,2).IsRequired();
            e.Property(x => x.Description).HasMaxLength(4000);
            e.Property(x => x.Urgency).HasMaxLength(30).IsRequired();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            e.Property(x => x.AssignedRole).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.CreatedByUserId, x.CreatedAt });
            e.HasIndex(x => new { x.AssignedRole, x.Status, x.CreatedAt });
            e.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
