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
        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var lastEmotion = recentNotes.FirstOrDefault()?.Emotion ?? EmotionType.Neutral;
        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);

        return new CompanionViewModel
        {
            LastEmotion = lastEmotion,
            PetMood = GetPetMood(lastEmotion),
            PetMessage = GetPetMessage(lastEmotion, totalNotes),
            RecentNotes = recentNotes,
            NotesCount = totalNotes
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
}
