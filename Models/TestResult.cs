using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class TestResult
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
        public virtual ApplicationUser User { get; set; } = null!;

        // -----------------------------
        // Связь с тестом
        // -----------------------------
        [Required]
        public int TestId { get; set; }

        [ForeignKey(nameof(TestId))]
        public virtual Test Test { get; set; } = null!;

        // -----------------------------
        // Метаданные
        // -----------------------------
        [Required]
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;

        // -----------------------------
        // Результаты
        // -----------------------------
        public int Score { get; set; }

        [StringLength(50)]
        public string? Level { get; set; }

        [StringLength(2000)]
        public string? Interpretation { get; set; }
    }
}
