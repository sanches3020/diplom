using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;

namespace Sofia.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SofiaDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();

        try
        {
            await DatabaseSeeder.SeedAsync(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbInitializer] Seed skipped: {ex.Message}");
        }

        await SeedAdmin(userManager, roleManager);
    }

    private static async Task SeedAdmin(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        var email = "admin@sofia.local";
        var password = "Admin123!"; // потом в env

        if (!await roleManager.RoleExistsAsync("admin"))
            await roleManager.CreateAsync(new ApplicationRole("admin"));

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "admin");
        }
    }
}