using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Stats;

public class ReportViewModel
{
    public object ReportData { get; set; } = null!;
    public string Format { get; set; } = "html";
}
