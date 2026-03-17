namespace Sofia.Web.DTO.Psychologist;

public class AddScheduleRequest
{
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
