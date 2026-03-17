namespace Sofia.Web.DTO.Companion;

public class CompanionStatusResponse
{
    public string PetMood { get; set; } = "";
    public string PetMessage { get; set; } = "";
    public string LastEmotion { get; set; } = "";
    public int NotesCount { get; set; }
}
