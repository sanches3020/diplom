using Sofia.Web.DTO.Complaints;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.ViewModels.Admin;

public class AdminComplaintsViewModel
{
    public List<ComplaintDto> Complaints { get; set; } = new();
    public ComplaintStatsDto Stats { get; set; } = new();
    public int? StatusFilter { get; set; }
}
