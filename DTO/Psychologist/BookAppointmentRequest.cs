namespace Sofia.Web.DTO.Psychologist;

public class BookAppointmentRequest
{
    public int PsychologistId { get; set; }
    public string AppointmentDate { get; set; } = "";
    public string? Notes { get; set; }
}
