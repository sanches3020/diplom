using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Settings;
using System.Text;
using System.Text.Json;

namespace Sofia.Web.Services;

public class SettingsService : ISettingsService
{
    private readonly SofiaDbContext _context;

    public SettingsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<SettingsIndexViewModel?> GetIndexAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);
        var totalGoals = await _context.Goals.CountAsync(g => g.UserId == userId);
        var completedGoals = await _context.Goals.CountAsync(g => g.UserId == userId && g.Status == GoalStatus.Completed);
        var sharedNotes = await _context.Notes.CountAsync(n => n.UserId == userId && n.ShareWithPsychologist);
        var pinnedNotes = await _context.Notes.CountAsync(n => n.UserId == userId && n.IsPinned);
        var totalEmotions = await _context.EmotionEntries.CountAsync(e => e.UserId == userId);

        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var recentGoals = await _context.Goals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .Take(3)
            .ToListAsync();

        return new SettingsIndexViewModel
        {
            User = user,
            TotalNotes = totalNotes,
            TotalGoals = totalGoals,
            CompletedGoals = completedGoals,
            SharedNotes = sharedNotes,
            PinnedNotes = pinnedNotes,
            TotalEmotions = totalEmotions,
            RecentNotes = recentNotes,
            RecentGoals = recentGoals
        };
    }

    public async Task<ProfileViewModel?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);
        var totalGoals = await _context.Goals.CountAsync(g => g.UserId == userId);
        var completedGoals = await _context.Goals.CountAsync(g => g.UserId == userId && g.Status == GoalStatus.Completed);
        var totalEmotions = await _context.EmotionEntries.CountAsync(e => e.UserId == userId);

        return new ProfileViewModel
        {
            User = user,
            TotalNotes = totalNotes,
            TotalGoals = totalGoals,
            CompletedGoals = completedGoals,
            TotalEmotions = totalEmotions
        };
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string name, string email, string bio)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return (false, "Пользователь не найден");

        user.FullName = name;
        user.Email = email;
        user.Bio = bio;

        await _context.SaveChangesAsync();
        return (true, "Профиль успешно обновлён");
    }

    public async Task<(bool Success, string Message, byte[]? Bytes, string? ContentType, string? FileName)> ExportDataAsync(int userId, string format)
    {
        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var goals = await _context.Goals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        var fileName = $"sofia_export_{DateTime.Now:yyyy-MM-dd}";
        if (format == "json")
        {
            var exportData = new
            {
                Notes = notes,
                Goals = goals,
                ExportDate = DateTime.Now,
                Version = "1.0"
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return (true, "OK", bytes, "application/json", fileName + ".json");
        }

        if (format == "csv")
        {
            var sb = new StringBuilder();
            sb.AppendLine("Тип,Дата,Содержание,Эмоция,Теги,Активность");

            foreach (var note in notes)
            {
                sb.AppendLine(
                    $"Заметка,{note.CreatedAt:yyyy-MM-dd HH:mm},\"{note.Content.Replace("\"", "\"\"")}\",{note.Emotion},{note.Tags ?? ""},{note.Activity ?? ""}");
            }

            foreach (var goal in goals)
            {
                sb.AppendLine(
                    $"Цель,{goal.CreatedAt:yyyy-MM-dd HH:mm},\"{goal.Title}\",{goal.Type},{goal.Status},{goal.Progress}%");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return (true, "OK", bytes, "text/csv", fileName + ".csv");
        }

        return (false, "Неподдерживаемый формат", null, null, null);
    }
}
