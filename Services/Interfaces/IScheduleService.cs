using Sofia.Web.DTO.Psychologist;
using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces;

public interface IScheduleService
{
    Task<ScheduleViewModel?> GetScheduleAsync(int psychologistUserId);

    Task<(bool Success, string Message)> AddScheduleAsync(int psychologistUserId, AddScheduleRequest request);
    Task<(bool Success, string Message)> RemoveScheduleAsync(int psychologistUserId, int scheduleId);

    Task<(bool Success, string Message, int SlotsCreated)> AddTimeSlotsAsync(int psychologistUserId, AddTimeSlotRequest request);
}
