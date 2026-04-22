using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Notes;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.Models;

namespace Sofia.Web.Services;

public class NotesService : INotesService
{
    private readonly SofiaDbContext _context;

    public NotesService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Note>> GetUserNotesAsync(string userId)
    {
        return await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Date)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> CreateNoteAsync(
        string userId,
        DTO.Notes.CreateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return (false, "Содержание заметки обязательно");

        var note = new Note
        {
            UserId = userId,
            Content = request.Content,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return (true, "Заметка успешно создана");
    }

    public async Task<(bool Success, string Message)> UpdateNoteAsync(string userId, int noteId, UpdateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return (false, "Содержание заметки обязательно");

        if (!Enum.TryParse<EmotionType>(request.Emotion, true, out var emotionType))
            return (false, "Неверный тип эмоции");

        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note == null)
            return (false, "Заметка не найдена");

        note.Content = request.Content;
        note.Tags = request.Tags;
        note.Emotion = emotionType;
        note.Activity = request.Activity;

        if (!string.IsNullOrEmpty(request.Date) &&
            DateTime.TryParse(request.Date, out var parsedDate))
        {
            note.Date = parsedDate;
        }

        note.IsPinned = request.IsPinned;
        note.ShareWithPsychologist = request.ShareWithPsychologist;
        note.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (true, "Заметка обновлена!");
    }

    public async Task<(bool Success, string Message)> DeleteNoteAsync(string userId, int noteId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note == null)
            return (false, "Заметка не найдена");

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return (true, "Заметка удалена!");
    }

    public async Task<(bool Success, string Message, bool? IsPinned)> TogglePinAsync(string userId, int noteId)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);

        if (note == null)
            return (false, "Заметка не найдена", null);

        note.IsPinned = !note.IsPinned;
        await _context.SaveChangesAsync();

        var message = note.IsPinned ? "Заметка закреплена!" : "Заметка откреплена!";
        return (true, message, note.IsPinned);
    }

    public async Task<(bool Success, int TodayNotes, int PinnedNotes, int SharedNotes)> GetStatsAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;

        var todayNotes = await _context.Notes
            .CountAsync(n => n.UserId == userId && n.Date.Date == today);

        var pinnedNotes = await _context.Notes
            .CountAsync(n => n.UserId == userId && n.IsPinned);

        var sharedNotes = await _context.Notes
            .CountAsync(n => n.UserId == userId && n.ShareWithPsychologist);

        return (true, todayNotes, pinnedNotes, sharedNotes);
    }

    public async Task<Note?> GetNoteAsync(string userId, int id)
    {
        return await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
    }
}
