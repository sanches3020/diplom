using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основной текст заметки
        // -----------------------------
        [Required(ErrorMessage = "Текст заметки обязателен")]
        [StringLength(2000, ErrorMessage = "Максимум 2000 символов")]
        public string Content { get; set; } = string.Empty;

        // -----------------------------
        // Теги (опционально)
        // -----------------------------
        [StringLength(500)]
        public string? Tags { get; set; }

        // -----------------------------
        // Эмоция (enum)
        // -----------------------------
        [Required]
        public EmotionType Emotion { get; set; }

        // -----------------------------
        // Деятельность (опционально)
        // -----------------------------
        [StringLength(500)]
        public string? Activity { get; set; }

        // -----------------------------
        // Дата, к которой относится заметка
        // -----------------------------
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // -----------------------------
        // Метаданные
        // -----------------------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsPinned { get; set; } = false;

        public bool ShareWithPsychologist { get; set; } = false;

        // -----------------------------
        // Связь с пользователем (Identity)
        // -----------------------------
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public enum EmotionType
    {
        VerySad = 1,
        Sad = 2,
        Neutral = 3,
        Happy = 4,
        VeryHappy = 5,
        Anxious = 6,
        Calm = 7,
        Excited = 8,
        Frustrated = 9,
        Grateful = 10
    }
}
