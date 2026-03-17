using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IClientAnalyticsService
{
    Task<PsychologistDashboardViewModel?> GetDashboardAsync(int psychologistUserId);
}
