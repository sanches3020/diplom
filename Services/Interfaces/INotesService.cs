using Sofia.Web.DTO.Notes;
using Sofia.Web.Models;

namespace Sofia.Web.Services.Interfaces;

public interface INotesService
{
    Task<List<Note>> GetUserNotesAsync(string userId);
<<<<<<< HEAD
    Task<(bool Success, string Message)> CreateNoteAsync(string userId, DTO.Notes.CreateNoteRequest request);
    Task<(bool Success, string Message)> UpdateNoteAsync(string userId, int noteId, UpdateNoteRequest request);
    Task<(bool Success, string Message)> DeleteNoteAsync(string userId, int noteId);
    Task<(bool Success, string Message, bool? IsPinned)> TogglePinAsync(string userId, int noteId);
    Task<(bool Success, int TodayNotes, int PinnedNotes, int SharedNotes)> GetStatsAsync(string userId);
    Task<Note> GetNoteAsync(string userId, int id);
=======
    Task<(bool Success, string Message)> CreateNoteAsync(string userId, Sofia.Web.DTO.Notes.CreateNoteRequest request);
    Task<(bool Success, string Message)> UpdateNoteAsync(string userId, int noteId, Sofia.Web.DTO.Notes.UpdateNoteRequest request);
    Task<(bool Success, string Message)> DeleteNoteAsync(string userId, int noteId);
    Task<(bool Success, string Message, bool? IsPinned)> TogglePinAsync(string userId, int noteId);
    Task<(bool Success, int TodayNotes, int PinnedNotes, int SharedNotes)> GetStatsAsync(string userId);
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab
}
