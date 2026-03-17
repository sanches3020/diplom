using Sofia.Web.ViewModels.Stats;

namespace Sofia.Web.Services.Interfaces;

public interface IStatsService
{
    Task<StatsIndexViewModel> GetStatsAsync(int userId, int daysBack);
    Task<byte[]> ExportCsvAsync(int userId, int daysBack);
    Task<InsightsViewModel> GetInsightsAsync(int userId);
    Task<ReportViewModel> GenerateReportAsync(int userId, int daysBack, string format);
}
