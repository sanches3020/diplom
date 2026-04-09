namespace Sofia.Web.Models;

public class MediaFile
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public DateTime UploadDate { get; set; }
}
