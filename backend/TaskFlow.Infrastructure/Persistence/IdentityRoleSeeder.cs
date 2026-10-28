using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Infrastructure.Persistence;

public static class IdentityRoleSeeder
{
    private static readonly string[] Roles =
    [
        "SystemAdmin",
        "User"
    ];

    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in Roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {errors}");
            }
        }
    }
}