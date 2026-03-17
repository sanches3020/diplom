using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class UserAnswer
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
        // Связь с вопросом
        // -----------------------------
        [Required]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; } = null!;

        // -----------------------------
        // Связь с вариантом ответа (если есть)
        // -----------------------------
        public int? AnswerId { get; set; }

        [ForeignKey(nameof(AnswerId))]
        public virtual Answer? Answer { get; set; }

        // -----------------------------
        // Свободный текст (для текстовых вопросов)
        // -----------------------------
        [StringLength(2000)]
        public string? TextAnswer { get; set; }

        // -----------------------------
        // Метаданные
        // -----------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
