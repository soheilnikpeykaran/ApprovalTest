using Approval.Application.Interfaces;
using Approval.Domain.Entities;
using Approval.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Approval.Infrastructure;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var hasher = services.GetRequiredService<IPasswordHasher>();
        await db.Database.MigrateAsync();
        foreach (var name in new[] { "Employee", "Manager", "Finance" })
            if (!await db.Roles.AnyAsync(x => x.Name == name)) db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = name });
        await db.SaveChangesAsync();
        await CreateUser(db, hasher, "employee@test.com", "Employee123!", "Employee");
        await CreateUser(db, hasher, "manager@test.com", "Manager123!", "Manager");
        await CreateUser(db, hasher, "finance@test.com", "Finance123!", "Finance");
        await db.SaveChangesAsync();
    }
    private static async Task CreateUser(ApplicationDbContext db, IPasswordHasher hasher, string email, string password, string roleName)
    {
        if (await db.Users.AnyAsync(x => x.Email == email)) return;
        var role = await db.Roles.SingleAsync(x => x.Name == roleName);
        var user = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = hasher.Hash(password), FirstName = roleName, LastName = "Test", IsActive = true, CreatedAt = DateTime.UtcNow };
        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, User = user, Role = role });
        db.Users.Add(user);
    }
}
