namespace Sofia.Web.DTO.Notes;

public class CreateNoteRequest
{
    public string Content { get; set; } = null!;
    public string Emotion { get; set; } = null!;
    public string? Tags { get; set; }
    public string? Activity { get; set; }
    public string? Date { get; set; }
    public bool IsPinned { get; set; }
    public bool ShareWithPsychologist { get; set; }
}
