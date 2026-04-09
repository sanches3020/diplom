using Sofia.Web.ViewModels.Psychologist;
using Sofia.Web.ViewModels.Psychologists;

namespace Sofia.Web.Services.Interfaces;

public interface IClientAnalyticsService
{
    Task<PsychologistDashboardViewModel?> GetDashboardAsync(string psychologistUserId);
}
