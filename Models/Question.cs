using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Question
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
        // Текст вопроса
        // -----------------------------
        [Required]
        [StringLength(1000)]
        public string Text { get; set; } = string.Empty;

        // -----------------------------
        // Тип вопроса
        // -----------------------------
        [Required]
        public AnswerType Type { get; set; } = AnswerType.SingleChoice;

        // -----------------------------
        // Навигация к ответам
        // -----------------------------
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
