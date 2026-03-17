using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class PsychologistSchedule
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
    // День недели
    // -----------------------------
    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    // -----------------------------
    // Время работы
    // -----------------------------
    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    // -----------------------------
    // Статус
    // -----------------------------
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
