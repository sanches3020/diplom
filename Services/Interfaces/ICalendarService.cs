using Sofia.Web.DTO.Calendar;
using Sofia.Web.ViewModels.Calendar;

namespace Sofia.Web.Services.Interfaces;

public interface ICalendarService
{
    Task<CalendarIndexViewModel> GetCalendarAsync(int userId, int? year, int? month);
    Task<(bool Success, string Message)> SaveEmotionAsync(int userId, SaveEmotionRequest request);
    Task<DayDetailsViewModel?> GetDayDetailsAsync(int userId, DateTime date);
    Task<(IEnumerable<object> EmotionStats, int DaysBack)> GetEmotionStatsAsync(int? days);
}
