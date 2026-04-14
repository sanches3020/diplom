using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class PsychologistReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Кто оставил отзыв
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        // К какому психологу относится отзыв
        [Required]
        public int PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist Psychologist { get; set; } = null!;

        // Заголовок отзыва (опционально)
        [StringLength(200)]
        public string? Title { get; set; }

        // Основной текст отзыва
        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        // Оценка (1–5)
        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        // Модерация
        public bool IsApproved { get; set; } = false;
        public bool IsVisible { get; set; } = true;

        // Даты
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
