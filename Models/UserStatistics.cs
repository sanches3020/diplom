using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class UserStatistics
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
        // Дата статистики (обычно день)
        // -----------------------------
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        // -----------------------------
        // Статистика по заметкам
        // -----------------------------
        public int NotesCount { get; set; } = 0;
        public int PinnedNotesCount { get; set; } = 0;
        public int SharedNotesCount { get; set; } = 0;

        // -----------------------------
        // Статистика по целям
        // -----------------------------
        public int GoalsCount { get; set; } = 0;
        public int CompletedGoalsCount { get; set; } = 0;
        public int ActiveGoalsCount { get; set; } = 0;

        [Range(0, 100)]
        public double AverageGoalProgress { get; set; } = 0;

        // -----------------------------
        // Статистика по эмоциям
        // -----------------------------
        public int EmotionsCount { get; set; } = 0;

        public EmotionType? DominantEmotion { get; set; }

        [Range(0, 100)]
        public double AverageEmotionScore { get; set; } = 0;

        // -----------------------------
        // Статистика по практикам
        // -----------------------------
        public int PracticesCount { get; set; } = 0;
        public int TotalPracticeMinutes { get; set; } = 0;

        // -----------------------------
        // Статистика по сессиям с психологом
        // -----------------------------
        public int AppointmentsCount { get; set; } = 0;
        public int CompletedAppointmentsCount { get; set; } = 0;

        // -----------------------------
        // Общая активность
        // -----------------------------
        public int TotalActivityDays { get; set; } = 0;
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;

        // -----------------------------
        // Метаданные
        // -----------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
