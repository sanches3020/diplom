using Sofia.Web.Data;
using Sofia.Web.Models;

namespace Sofia.Web.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SofiaDbContext context)
    {
        // Если нужно — добавь тестовые данные
        // Пример:
        // if (!context.Psychologists.Any())
        // {
        //     context.Psychologists.Add(new Psychologist { FullName = "Тестовый психолог" });
        //     await context.SaveChangesAsync();
        // }
    }
}
