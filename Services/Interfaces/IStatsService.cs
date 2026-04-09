using Sofia.Web.ViewModels.Stats;

namespace Sofia.Web.Services.Interfaces;

public interface IStatsService
{
    Task<StatsIndexViewModel> GetStatsAsync(string userId, int daysBack);
    Task<byte[]> ExportCsvAsync(string userId, int daysBack);
    Task<InsightsViewModel> GetInsightsAsync(string userId);
    Task<ReportViewModel> GenerateReportAsync(string userId, int daysBack, string format);
}
