using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services;

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

        await SeedRoles(roleManager);
        await SeedAdmin(userManager, roleManager);
        await SeedDemoDataAsync(context, userManager, roleManager, scope.ServiceProvider.GetService<ChatStorage>());
    }

    private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[] { "user", "psychologist", "admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }
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

    private static async Task SeedDemoDataAsync(
        SofiaDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ChatStorage? chatStorage)
    {
        var demoPassword = "Demo123!";

        var users = new[]
        {
            ("anna.user@sofia.local", "Анна Петрова", "user"),
            ("max.user@sofia.local", "Максим Иванов", "user")
        };

        var psychologists = new[]
        {
            ("irina.psy@sofia.local", "Ирина Орлова", "psychologist", "Тревожность и самооценка"),
            ("dmitry.psy@sofia.local", "Дмитрий Смирнов", "psychologist", "Эмоциональное выгорание")
        };

        var userEntities = new List<ApplicationUser>();
        foreach (var (email, fullName, role) in users)
        {
            var user = await EnsureUserAsync(userManager, roleManager, email, fullName, role, demoPassword);
            userEntities.Add(user);
        }

        var psychologistUsers = new List<ApplicationUser>();
        foreach (var (email, fullName, role, specialization) in psychologists)
        {
            var user = await EnsureUserAsync(userManager, roleManager, email, fullName, role, demoPassword);
            psychologistUsers.Add(user);

            if (!await context.Psychologists.AnyAsync(p => p.UserId == user.Id))
            {
                context.Psychologists.Add(new Psychologist
                {
                    Name = fullName,
                    UserId = user.Id,
                    Specialization = specialization,
                    Description = $"Практикующий психолог: {specialization}.",
                    Experience = "5+ лет",
                    Languages = "русский",
                    Methods = "КПТ, mindfulness",
                    ContactEmail = email,
                    IsActive = true,
                    PricePerHour = 170m
                });
            }
        }

        await context.SaveChangesAsync();

        await SeedPracticesAsync(context);
        await SeedUserContentAsync(context, userEntities, psychologistUsers);
        await SeedForumAsync(context, userEntities, psychologistUsers);
        SeedChat(chatStorage, userEntities, psychologistUsers);
        await context.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        string email,
        string fullName,
        string role,
        string password)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new ApplicationRole(role));
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                UserType = role,
                CompanionJoinDate = DateTime.UtcNow.AddDays(-14)
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Не удалось создать пользователя {email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            var updated = false;
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                user.FullName = fullName;
                updated = true;
            }
            if (string.IsNullOrWhiteSpace(user.UserType))
            {
                user.UserType = role;
                updated = true;
            }
            if (updated)
            {
                await userManager.UpdateAsync(user);
            }
        }

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains(role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    private static async Task SeedPracticesAsync(SofiaDbContext context)
    {
        if (await context.Practices.AnyAsync())
        {
            return;
        }

        context.Practices.AddRange(
            new Practice
            {
                Name = "Квадратное дыхание",
                Description = "Стабилизация состояния через ритмичное дыхание",
                Category = PracticeCategory.Breathing,
                DurationMinutes = 5,
                Instructions = "Вдох 4 сек, пауза 4 сек, выдох 4 сек, пауза 4 сек."
            },
            new Practice
            {
                Name = "Сканирование тела",
                Description = "Осознанное расслабление тела по зонам",
                Category = PracticeCategory.Relaxation,
                DurationMinutes = 10,
                Instructions = "Постепенно переводите внимание от стоп к макушке."
            },
            new Practice
            {
                Name = "Дневник мыслей",
                Description = "Фиксация автоматических мыслей и их проверка",
                Category = PracticeCategory.CBT,
                DurationMinutes = 15,
                Instructions = "Запишите ситуацию, мысль, эмоцию и альтернативную мысль."
            });
    }

    private static async Task SeedUserContentAsync(
        SofiaDbContext context,
        List<ApplicationUser> users,
        List<ApplicationUser> psychologistUsers)
    {
        var psychologistId = psychologistUsers.FirstOrDefault()?.Id;

        foreach (var user in users)
        {
            if (!await context.Notes.AnyAsync(n => n.UserId == user.Id))
            {
                context.Notes.AddRange(
                    new Note
                    {
                        UserId = user.Id,
                        Content = "Сегодня было тревожно перед учебой, помогла короткая прогулка.",
                        Emotion = EmotionType.Anxious,
                        Tags = "учеба,тревога",
                        Date = DateTime.UtcNow.Date.AddDays(-2),
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Note
                    {
                        UserId = user.Id,
                        Content = "Сделал дыхательную практику и почувствовал спокойствие.",
                        Emotion = EmotionType.Calm,
                        Tags = "практика,дыхание",
                        Date = DateTime.UtcNow.Date.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });
            }

            if (!await context.EmotionEntries.AnyAsync(e => e.UserId == user.Id))
            {
                context.EmotionEntries.AddRange(
                    new EmotionEntry
                    {
                        UserId = user.Id,
                        Date = DateTime.UtcNow.Date.AddDays(-2),
                        Emotion = EmotionType.Anxious,
                        Note = "Нагрузка перед дедлайном",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new EmotionEntry
                    {
                        UserId = user.Id,
                        Date = DateTime.UtcNow.Date.AddDays(-1),
                        Emotion = EmotionType.Calm,
                        Note = "Удалось выделить время на отдых",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });
            }

            if (!await context.Goals.AnyAsync(g => g.UserId == user.Id))
            {
                context.Goals.AddRange(
                    new Goal
                    {
                        UserId = user.Id,
                        Title = "Стабилизировать режим сна",
                        Description = "Ложиться до 23:30 минимум 5 дней в неделю",
                        Type = GoalType.Wellness,
                        Status = GoalStatus.Active,
                        Progress = 35,
                        Date = DateTime.UtcNow.Date,
                        TargetDate = DateTime.UtcNow.Date.AddDays(21),
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        PsychologistId = psychologistId,
                        IsFromPsychologist = psychologistId != null
                    },
                    new Goal
                    {
                        UserId = user.Id,
                        Title = "Снизить тревожность перед учебой",
                        Description = "Ежедневно делать 10 минут дыхательной практики",
                        Type = GoalType.Therapy,
                        Status = GoalStatus.Active,
                        Progress = 50,
                        Date = DateTime.UtcNow.Date.AddDays(-3),
                        TargetDate = DateTime.UtcNow.Date.AddDays(14),
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    });
            }
        }
    }

    private static async Task SeedForumAsync(
        SofiaDbContext context,
        List<ApplicationUser> users,
        List<ApplicationUser> psychologistUsers)
    {
        if (!await context.ForumCategories.AnyAsync())
        {
            context.ForumCategories.AddRange(
                new ForumCategory { Title = "Поддержка и мотивация", Description = "Обсуждения ежедневных трудностей и поддержки" },
                new ForumCategory { Title = "Практики и техники", Description = "Дыхание, mindfulness и полезные упражнения" });
            await context.SaveChangesAsync();
        }

        if (await context.ForumThreads.AnyAsync())
        {
            return;
        }

        var author = users.FirstOrDefault();
        var psychologist = psychologistUsers.FirstOrDefault();
        var category = await context.ForumCategories.FirstAsync();
        if (author == null || psychologist == null)
        {
            return;
        }

        var thread = new ForumThread
        {
            Title = "Как вы справляетесь с тревогой перед экзаменом?",
            CategoryId = category.Id,
            AuthorId = author.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        context.ForumThreads.Add(thread);
        await context.SaveChangesAsync();

        context.ForumPosts.AddRange(
            new ForumPost
            {
                ThreadId = thread.Id,
                AuthorId = author.Id,
                Content = "Мне помогает техника 4-7-8 и короткая прогулка перед началом подготовки.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ForumPost
            {
                ThreadId = thread.Id,
                AuthorId = psychologist.Id,
                Content = "Отличная практика. Еще можно добавить заземление 5-4-3-2-1.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
    }

    private static void SeedChat(
        ChatStorage? chatStorage,
        List<ApplicationUser> users,
        List<ApplicationUser> psychologistUsers)
    {
        if (chatStorage == null)
        {
            return;
        }

        if (chatStorage.GetMessages("general").Any())
        {
            return;
        }

        var firstUser = users.FirstOrDefault();
        var firstPsychologist = psychologistUsers.FirstOrDefault();

        if (firstUser != null)
        {
            chatStorage.AddMessage("general", new ChatMessage
            {
                Room = "general",
                UserId = firstUser.Id,
                UserName = firstUser.FullName ?? "Пользователь",
                AvatarCode = "blue:UP",
                Text = "Привет всем! Кто делает вечерние практики?",
                CreatedAt = DateTime.UtcNow.AddMinutes(-20)
            });
        }

        if (firstPsychologist != null)
        {
            chatStorage.AddMessage("general", new ChatMessage
            {
                Room = "general",
                UserId = firstPsychologist.Id,
                UserName = firstPsychologist.FullName ?? "Психолог",
                AvatarCode = "green:PS",
                Text = "Добрый вечер! Попробуйте начать с 5 минут спокойного дыхания.",
                CreatedAt = DateTime.UtcNow.AddMinutes(-15)
            });
        }
    }
}