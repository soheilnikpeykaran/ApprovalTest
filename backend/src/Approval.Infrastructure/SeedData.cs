using Approval.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Approval.Infrastructure;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "Employee", "Manager", "Finance" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            "; ",
                            result.Errors.Select(x => x.Description)));
                }
            }
        }

        await EnsureUser(
            userManager,
            "employee@test.com",
            "Employee123!",
            "Test",
            "Employee",
            "Employee");

        await EnsureUser(
            userManager,
            "manager@test.com",
            "Manager123!",
            "Test",
            "Manager",
            "Manager");

        await EnsureUser(
            userManager,
            "finance@test.com",
            "Finance123!",
            "Test",
            "Finance",
            "Finance");
    }

    private static async Task EnsureUser(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string first,
        string last,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = first,
                LastName = last,
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        result.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var result =
                await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        result.Errors.Select(x => x.Description)));
            }
        }
    }
}