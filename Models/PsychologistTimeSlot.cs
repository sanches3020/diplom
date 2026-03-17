using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class PsychologistTimeSlot
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // -----------------------------
    // Связь с психологом
    // -----------------------------
    [Required]
    public int PsychologistId { get; set; }

    [ForeignKey(nameof(PsychologistId))]
    public virtual Psychologist Psychologist { get; set; } = null!;

    // -----------------------------
    // Дата и время слота
    // -----------------------------
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    // -----------------------------
    // Статус
    // -----------------------------
    public bool IsAvailable { get; set; } = true;

    public bool IsBooked { get; set; } = false;

    // -----------------------------
    // Кто забронировал (Identity)
    // -----------------------------
    public string? BookedByUserId { get; set; }

    [ForeignKey(nameof(BookedByUserId))]
    public virtual ApplicationUser? BookedByUser { get; set; }

    // -----------------------------
    // Метаданные
    // -----------------------------
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
