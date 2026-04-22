using Sofia.Web.Data;
using Sofia.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Sofia.Web.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SofiaDbContext context)
    {
        await SeedUsersAsync(context);
        await SeedPsychologistsAsync(context);
        await SeedPracticesAsync(context);
        await SeedTestsAsync(context);
        await SeedForumAsync(context);
        await SeedEmotionsAndNotesAsync(context);
    }

    private static async Task SeedUsersAsync(SofiaDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Email != null && u.Email.StartsWith("seed-user")))
            return;

        var users = Enumerable.Range(1, 6)
            .Select(i => new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = $"seed-user{i}@sofia.local",
                NormalizedUserName = $"SEED-USER{i}@SOFIA.LOCAL",
                Email = $"seed-user{i}@sofia.local",
                NormalizedEmail = $"SEED-USER{i}@SOFIA.LOCAL",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                FullName = $"Тестовый Пользователь {i}",
                UserType = i <= 2 ? "psychologist" : "user",
                CreatedAt = DateTime.UtcNow.AddDays(-20 + i),
                CompanionJoinDate = DateTime.UtcNow.AddDays(-10 + i),
                CompanionLevel = Math.Min(5, i)
            })
            .ToList();

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPsychologistsAsync(SofiaDbContext context)
    {
        if (await context.Psychologists.AnyAsync())
            return;

        var specs = new[]
        {
            "КПТ", "Семейная терапия", "Работа с тревогой", "Детская психология",
            "Кризисное консультирование", "Гештальт", "ПТСР", "Психосоматика",
            "Эмоциональное выгорание", "Подростковая психология", "Mindfulness", "Коучинг"
        };

        var seedUsers = await context.Users
            .Where(u => u.Email != null && u.Email.StartsWith("seed-user"))
            .OrderBy(u => u.Email)
            .ToListAsync();

        var psychologists = Enumerable.Range(1, 12)
            .Select(i => new Psychologist
            {
                Name = $"Психолог {i}",
                UserId = seedUsers[(i - 1) % seedUsers.Count].Id,
                Specialization = specs[(i - 1) % specs.Length],
                Description = $"Практикующий специалист №{i} с фокусом на {specs[(i - 1) % specs.Length].ToLowerInvariant()}.",
                Education = "МГУ, факультет психологии",
                Experience = $"{3 + i} лет практики",
                Languages = "Русский, English",
                Methods = "КПТ, майндфулнесс, поддерживающая терапия",
                PhotoUrl = "/images/psychologists/default.png",
                PricePerHour = 1800 + i * 100,
                ContactEmail = $"psych{i}@sofia.local",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30 + i)
            })
            .ToList();

        await context.Psychologists.AddRangeAsync(psychologists);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPracticesAsync(SofiaDbContext context)
    {
        if (await context.Practices.AnyAsync())
            return;

        var practices = Enumerable.Range(1, 12)
            .Select(i => new Practice
            {
                Name = $"Практика #{i}",
                Description = "Короткая техника для снижения стресса и стабилизации состояния.",
                Category = (PracticeCategory)((i % 7) + 1),
                DurationMinutes = 5 + (i % 4) * 5,
                Instructions = "Сконцентрируйтесь на дыхании и выполните шаги по инструкции.",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15 + i)
            })
            .ToList();

        await context.Practices.AddRangeAsync(practices);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTestsAsync(SofiaDbContext context)
    {
        if (await context.Tests.AnyAsync())
            return;

        var tests = Enumerable.Range(1, 6)
            .Select(i => new Test
            {
                Name = $"Психологический тест #{i}",
                Description = "Оценка текущего состояния и факторов стресса.",
                Type = TestType.BuiltIn,
                CreatedAt = DateTime.UtcNow.AddDays(-20 + i)
            })
            .ToList();

        await context.Tests.AddRangeAsync(tests);
        await context.SaveChangesAsync();

        var questions = new List<Question>();
        foreach (var test in tests)
        {
            for (var q = 1; q <= 4; q++)
            {
                questions.Add(new Question
                {
                    TestId = test.Id,
                    Text = $"Вопрос {q} для теста \"{test.Name}\"",
                    Type = AnswerType.SingleChoice
                });
            }
        }

        await context.Questions.AddRangeAsync(questions);
        await context.SaveChangesAsync();

        var answers = new List<Answer>();
        foreach (var question in questions)
        {
            for (var a = 1; a <= 4; a++)
            {
                answers.Add(new Answer
                {
                    QuestionId = question.Id,
                    Text = $"Вариант {a}",
                    Value = a,
                    Order = a
                });
            }
        }

        await context.Answers.AddRangeAsync(answers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedForumAsync(SofiaDbContext context)
    {
        if (await context.ForumCategories.AnyAsync())
            return;

        var users = await context.Users
            .Where(u => u.Email != null && u.Email.StartsWith("seed-user"))
            .Take(4)
            .ToListAsync();

        var categories = new List<ForumCategory>
        {
            new() { Title = "Тревожность", Description = "Обсуждение тревожных состояний" },
            new() { Title = "Отношения", Description = "Поддержка и советы по отношениям" },
            new() { Title = "Саморазвитие", Description = "Привычки, цели, рост" }
        };
        await context.ForumCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var threads = new List<ForumThread>();
        foreach (var category in categories)
        {
            for (var i = 1; i <= 2; i++)
            {
                threads.Add(new ForumThread
                {
                    CategoryId = category.Id,
                    Title = $"{category.Title}: тема {i}",
                    AuthorId = users[(i - 1) % users.Count].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }
        }
        await context.ForumThreads.AddRangeAsync(threads);
        await context.SaveChangesAsync();

        var posts = new List<ForumPost>();
        foreach (var thread in threads)
        {
            for (var i = 1; i <= 3; i++)
            {
                posts.Add(new ForumPost
                {
                    ThreadId = thread.Id,
                    AuthorId = users[(i - 1) % users.Count].Id,
                    Content = $"Пост {i} в теме \"{thread.Title}\"",
                    CreatedAt = DateTime.UtcNow.AddHours(-i)
                });
            }
        }
        await context.ForumPosts.AddRangeAsync(posts);
        await context.SaveChangesAsync();

        // NOTE: like seeding is intentionally skipped for now because
        // current relationship mapping in the project creates a shadow FK
        // column (UserId1) in forum.PostLikes and breaks startup seeding.
    }

    private static async Task SeedEmotionsAndNotesAsync(SofiaDbContext context)
    {
        if (await context.EmotionEntries.AnyAsync() || await context.Notes.AnyAsync())
            return;

        var users = await context.Users
            .Where(u => u.Email != null && u.Email.StartsWith("seed-user"))
            .Take(3)
            .ToListAsync();

        var emotions = new[] { EmotionType.Happy, EmotionType.Calm, EmotionType.Anxious, EmotionType.Grateful };
        var notes = new List<Note>();
        var emotionEntries = new List<EmotionEntry>();

        foreach (var user in users)
        {
            for (var i = 0; i < 6; i++)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                var emotion = emotions[i % emotions.Length];

                notes.Add(new Note
                {
                    UserId = user.Id,
                    Content = $"Тестовая заметка {i + 1} пользователя {user.UserName}",
                    Tags = "стресс,сон,работа",
                    Emotion = emotion,
                    Activity = i % 2 == 0 ? "Прогулка" : "Чтение",
                    Date = date,
                    CreatedAt = date.AddHours(9 + i),
                    ShareWithPsychologist = i % 3 == 0
                });

                emotionEntries.Add(new EmotionEntry
                {
                    UserId = user.Id,
                    Date = date,
                    Emotion = emotion,
                    Note = $"Запись эмоции {i + 1}",
                    CreatedAt = date.AddHours(8 + i)
                });
            }
        }

        await context.Notes.AddRangeAsync(notes);
        await context.EmotionEntries.AddRangeAsync(emotionEntries);
        await context.SaveChangesAsync();
    }
}
