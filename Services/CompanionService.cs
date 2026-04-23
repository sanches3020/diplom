using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Companion;
using Sofia.Web.Models;
using Sofia.Web.Services.Companion;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Companion;

namespace Sofia.Web.Services;

public class CompanionService : ICompanionService
{
    private readonly SofiaDbContext _context;

    public CompanionService(SofiaDbContext context)
    {
        _context = context;
    }

    // -------------------------------------------------------
    // Основные данные компаньона
    // -------------------------------------------------------
    public async Task<CompanionViewModel> GetCompanionDataAsync(string userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.CompanionLevel })
            .FirstOrDefaultAsync();

        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var lastEmotion = recentNotes.FirstOrDefault()?.Emotion ?? EmotionType.Neutral;
        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);

        // Рассчитываем happiness на основе последней эмоции
        var happiness = lastEmotion switch
        {
            EmotionType.VeryHappy => 90,
            EmotionType.Happy => 80,
            EmotionType.Excited => 85,
            EmotionType.Calm => 70,
            EmotionType.Grateful => 80,
            EmotionType.Neutral => 50,
            EmotionType.Anxious => 40,
            EmotionType.Frustrated => 35,
            EmotionType.Sad => 30,
            EmotionType.VerySad => 20,
            _ => 50
        };

        return new CompanionViewModel
        {
            LastEmotion = lastEmotion,
            PetMood = GetPetMood(lastEmotion),
            PetMessage = GetPetMessage(lastEmotion, totalNotes),
            RecentNotes = recentNotes,
            NotesCount = totalNotes,
            CompanionLevel = user?.CompanionLevel ?? 1,
            Happiness = happiness,
            Energy = 50,
            Comfort = 50
        };
    }

    public async Task<CompanionStatusResponse> GetStatusAsync(string userId)
    {
        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(3)
            .ToListAsync();

        var lastEmotion = recentNotes.FirstOrDefault()?.Emotion ?? EmotionType.Neutral;
        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);

        return new CompanionStatusResponse
        {
            PetMood = GetPetMood(lastEmotion),
            PetMessage = GetPetMessage(lastEmotion, totalNotes),
            LastEmotion = lastEmotion.ToString(),
            NotesCount = totalNotes
        };
    }

    // -------------------------------------------------------
    // Действия компаньона (ASYNC)
    // -------------------------------------------------------
    public Task<(int Value, string Message, string Mood)> FeedAsync(string userId)
    {
        var happiness = Random.Shared.Next(70, 100);
        return Task.FromResult((happiness, CompanionMessages.GetFeedMessage(), "happy"));
    }

    public Task<(int Value, string Message, string Mood)> PlayAsync(string userId)
    {
        var energy = Random.Shared.Next(60, 90);
        return Task.FromResult((energy, CompanionMessages.GetPlayMessage(), "excited"));
    }

    public Task<(int Value, string Message, string Mood)> ComfortAsync(string userId)
    {
        var comfort = Random.Shared.Next(80, 100);
        return Task.FromResult((comfort, CompanionMessages.GetComfortMessage(), "calm"));
    }

    // -------------------------------------------------------
    // PRIVATE HELPERS
    // -------------------------------------------------------
    private string GetPetMood(EmotionType emotion) =>
        CompanionMessages.EmotionMoodMap.TryGetValue(emotion, out var mood)
            ? mood
            : "neutral";

    private string GetPetMessage(EmotionType emotion, int notesCount)
    {
        if (CompanionMessages.EmotionMessages.TryGetValue(emotion, out var messages))
            return messages[Random.Shared.Next(messages.Length)];

        return "🐾 Привет! Как дела?";
    }

    // ===== Companion Level System =====

    /// <summary>
    /// Получить уровень компаньена на основе дней в системе
    /// 0–2 → уровень 1
    /// 3–9 → уровень 2
    /// 10–29 → уровень 3
    /// 30–59 → уровень 4
    /// 60+ → уровень 5
    /// </summary>
    private int CalculateCompanionLevel(int daysInSystem)
    {
        return daysInSystem switch
        {
            <= 2 => 1,
            <= 9 => 2,
            <= 29 => 3,
            <= 59 => 4,
            _ => 5
        };
    }

    /// <summary>
    /// Получить информацию о уровне компаньена пользователя
    /// </summary>
    public async Task<CompanionLevelInfoResponse> GetCompanionLevelInfoAsync(string userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.CompanionJoinDate, u.CompanionLevel, u.CreatedAt })
            .FirstOrDefaultAsync();

        if (user == null)
            return new CompanionLevelInfoResponse { CurrentLevel = 1, DaysInSystem = 0, DaysToNextLevel = 3 };

        // Если CompanionJoinDate не установлена, используем дату регистрации
        var joinDate = user.CompanionJoinDate ?? user.CreatedAt;
        var daysInSystem = (int)(DateTime.UtcNow - joinDate).TotalDays;

        // Пересчитываем уровень во время запроса для проверки повышения уровня
        var currentLevel = CalculateCompanionLevel(daysInSystem);

        // Если уровень повысился, обновляем в БД
        if (currentLevel > user.CompanionLevel)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity != null)
            {
                userEntity.CompanionLevel = currentLevel;
                await _context.SaveChangesAsync();
            }
        }

        // Рассчитываем оставшиеся дни до следующего уровня
        int daysToNextLevel = currentLevel switch
        {
            1 => Math.Max(0, 3 - daysInSystem),
            2 => Math.Max(0, 10 - daysInSystem),
            3 => Math.Max(0, 30 - daysInSystem),
            4 => Math.Max(0, 60 - daysInSystem),
            _ => 0 // Максимальный уровень
        };

        // Рассчитываем процент прогресса к следующему уровню
        int progressPercent = currentLevel switch
        {
            1 => (daysInSystem * 100) / 3,
            2 => ((daysInSystem - 3) * 100) / 7,
            3 => ((daysInSystem - 10) * 100) / 20,
            4 => ((daysInSystem - 30) * 100) / 30,
            _ => 100 // Максимальный уровень
        };
        progressPercent = Math.Clamp(progressPercent, 0, 100);

        return new CompanionLevelInfoResponse
        {
            CurrentLevel = currentLevel,
            DaysInSystem = daysInSystem,
            DaysToNextLevel = daysToNextLevel,
            ProgressPercent = progressPercent,
            MaxLevel = currentLevel == 5
        };
    }

    /// <summary>
    /// Инициализировать дату присоединения компаньена для пользователя
    /// </summary>
    public async Task<bool> InitializeCompanionJoinDateAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // Если уже инициализирована, не перезаписываем
        if (user.CompanionJoinDate != null)
            return false;

        user.CompanionJoinDate = DateTime.UtcNow;
        user.CompanionLevel = 1;
        await _context.SaveChangesAsync();
        return true;
    }
}
