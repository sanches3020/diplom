using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class ScheduleViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistSchedule> Schedules { get; set; } = new();
    public List<PsychologistTimeSlot> ExistingSlots { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
