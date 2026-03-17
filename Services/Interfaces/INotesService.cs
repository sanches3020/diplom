using Sofia.Web.DTO.Notes;
using Sofia.Web.Models;

namespace Sofia.Web.Services.Interfaces;

public interface INotesService
{
    Task<List<Note>> GetUserNotesAsync(int userId);
    Task<(bool Success, string Message)> CreateNoteAsync(int userId, CreateNoteRequest request);
    Task<(bool Success, string Message)> UpdateNoteAsync(int userId, int noteId, UpdateNoteRequest request);
    Task<(bool Success, string Message)> DeleteNoteAsync(int userId, int noteId);
    Task<(bool Success, string Message, bool? IsPinned)> TogglePinAsync(int userId, int noteId);
    Task<(bool Success, int TodayNotes, int PinnedNotes, int SharedNotes)> GetStatsAsync(int userId);
}
