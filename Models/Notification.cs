using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основной текст уведомления
        // -----------------------------
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Message { get; set; }

        // -----------------------------
        // Тип и приоритет
        // -----------------------------
        [Required]
        public NotificationType Type { get; set; } = NotificationType.System;

        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        // -----------------------------
        // Статус
        // -----------------------------
        public bool IsRead { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // -----------------------------
        // Даты
        // -----------------------------
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ScheduledAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        // -----------------------------
        // Действие (кнопка)
        // -----------------------------
        [StringLength(100)]
        public string? ActionUrl { get; set; }

        [StringLength(50)]
        public string? ActionText { get; set; }

        // -----------------------------
        // Связь с пользователем (Identity)
        // -----------------------------
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
