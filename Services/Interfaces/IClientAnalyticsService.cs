using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces;

public interface IClientAnalyticsService
{
    Task<PsychologistDashboardViewModel?> GetDashboardAsync(string psychologistUserId);
}
