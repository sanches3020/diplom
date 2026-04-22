using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Goal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основная информация
        // -----------------------------
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public GoalType Type { get; set; } = GoalType.Personal;

        [Required]
        public GoalStatus Status { get; set; } = GoalStatus.Active;

        // -----------------------------
        // Даты
        // -----------------------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата, к которой относится цель (например, день создания или день выполнения)
        /// </summary>
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Целевая дата (опционально)
        /// </summary>
        public DateTime? TargetDate { get; set; }

        // -----------------------------
        // Прогресс
        // -----------------------------
        [Range(0, 100)]
        public int Progress { get; set; } = 0;

        public bool IsFromPsychologist { get; set; } = false;

        // -----------------------------
        // Связь с пользователем (Identity)
        // -----------------------------
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        // Psychologist who assigned this goal (optional)
        public string? PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual ApplicationUser? Psychologist { get; set; }
    }

    public enum GoalType
    {
        Personal = 1,
        Therapy = 2,
        Wellness = 3,
        Social = 4,
        Professional = 5
    }

    public enum GoalStatus
    {
        Active = 1,
        Completed = 2,
        Paused = 3,
        Cancelled = 4
    }
}
