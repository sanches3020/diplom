namespace Sofia.Web.ViewModels.Calendar;

public class CalendarIndexViewModel
{
    public DateTime CurrentMonth { get; set; }
    public DateTime PreviousMonth { get; set; }
    public DateTime NextMonth { get; set; }
    public List<CalendarDayViewModel> Days { get; set; } = new();
}
