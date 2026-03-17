using System.ComponentModel.DataAnnotations;

namespace Sofia.Web.Models;

public class CreateNoteRequest
{
    [Required]
    [StringLength(3000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Tags { get; set; }

    [Required]
    [StringLength(100)]
    public string Emotion { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Activity { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsPinned { get; set; } = false;

    public bool ShareWithPsychologist { get; set; } = false;
}
