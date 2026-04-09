using Sofia.Web.DTO.Psychologist;
using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IScheduleService
{
    Task<ScheduleViewModel?> GetScheduleAsync(string psychologistUserId);

    Task<(bool Success, string Message)> AddScheduleAsync(string psychologistUserId, AddScheduleRequest request);
    Task<(bool Success, string Message)> RemoveScheduleAsync(string psychologistUserId, int scheduleId);

    Task<(bool Success, string Message, int SlotsCreated)> AddTimeSlotsAsync(string psychologistUserId, AddTimeSlotRequest request);
}
