using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основная информация
        // -----------------------------
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public TestType Type { get; set; } = TestType.BuiltIn;

        // -----------------------------
        // Кто создал тест (психолог)
        // -----------------------------
        public int? CreatedByPsychologistId { get; set; }

        [ForeignKey(nameof(CreatedByPsychologistId))]
        public virtual Psychologist? CreatedByPsychologist { get; set; }

        // -----------------------------
        // Навигация к вопросам
        // -----------------------------
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

        // -----------------------------
        // Метаданные
        // -----------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
