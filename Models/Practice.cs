using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Practice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основная информация
        // -----------------------------
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public PracticeCategory Category { get; set; } = PracticeCategory.Mindfulness;

        // -----------------------------
        // Длительность
        // -----------------------------
        [Range(1, 300)]
        public int DurationMinutes { get; set; } = 5;

        // -----------------------------
        // Инструкции
        // -----------------------------
        [Column(TypeName = "text")]
        public string? Instructions { get; set; }

        // -----------------------------
        // Статус
        // -----------------------------
        public bool IsActive { get; set; } = true;

        // -----------------------------
        // Метаданные
        // -----------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public enum PracticeCategory
    {
        Breathing = 1,
        Visualization = 2,
        CBT = 3,
        Gestalt = 4,
        Meditation = 5,
        Mindfulness = 6,
        Relaxation = 7
    }
}
