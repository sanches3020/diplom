namespace Sofia.Web.DTO.Psychologist;

public class BookAppointmentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int? AppointmentId { get; set; }
    public DateTime? AppointmentDate { get; set; }
}
