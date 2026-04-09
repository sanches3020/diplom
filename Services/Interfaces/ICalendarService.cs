using Sofia.Web.DTO.Calendar;
using Sofia.Web.ViewModels.Calendar;

namespace Sofia.Web.Services.Interfaces;

public interface ICalendarService
{
    Task<CalendarIndexViewModel> GetCalendarAsync(string userId, int? year, int? month);
    Task<(bool Success, string Message)> SaveEmotionAsync(string userId, SaveEmotionRequest request);
    Task<DayDetailsViewModel?> GetDayDetailsAsync(string userId, DateTime date);
    Task<(IEnumerable<object> EmotionStats, int DaysBack)> GetEmotionStatsAsync(string userId, int? days);
}
