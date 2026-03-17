using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class TestInterpretation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Связь с тестом
        // -----------------------------
        [Required]
        public int TestId { get; set; }

        [ForeignKey(nameof(TestId))]
        public virtual Test Test { get; set; } = null!;

        // -----------------------------
        // Диапазон процентов (0–100)
        // -----------------------------
        [Required]
        [Range(0, 100)]
        public double MinPercent { get; set; }

        [Required]
        [Range(0, 100)]
        public double MaxPercent { get; set; }

        // -----------------------------
        // Уровень интерпретации
        // -----------------------------
        [Required]
        [StringLength(200)]
        public string Level { get; set; } = string.Empty;

        // -----------------------------
        // Текст интерпретации
        // -----------------------------
        [StringLength(2000)]
        public string? InterpretationText { get; set; }

        // -----------------------------
        // Метаданные
        // -----------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
