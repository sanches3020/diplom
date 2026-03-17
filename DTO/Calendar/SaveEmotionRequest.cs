namespace Sofia.Web.DTO.Calendar;

public class SaveEmotionRequest
{
    public string Date { get; set; } = "";
    public string Emotion { get; set; } = "";
    public string? Note { get; set; }
}
