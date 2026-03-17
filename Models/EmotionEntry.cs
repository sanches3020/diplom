using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class EmotionEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // -----------------------------
    // Связь с пользователем (Identity)
    // -----------------------------
    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    // -----------------------------
    // Дата, к которой относится эмоция
    // -----------------------------
    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    // -----------------------------
    // Тип эмоции (enum)
    // -----------------------------
    [Required]
    public EmotionType Emotion { get; set; }

    // -----------------------------
    // Дополнительная заметка
    // -----------------------------
    [StringLength(1000)]
    public string? Note { get; set; }

    // -----------------------------
    // Когда запись создана
    // -----------------------------
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
