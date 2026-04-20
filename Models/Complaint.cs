using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    /// <summary>
    /// Статусы жалобы
    /// </summary>
    public enum ComplaintStatus
    {
        New = 0,          // Новая жалоба
        Reviewed = 1,     // Просмотрена администратором
        Resolved = 2,     // Разрешена (принято действие)
        Dismissed = 3     // Отклонена (нарушения не найдено)
    }

    /// <summary>
    /// Причины жалобы
    /// </summary>
    public enum ComplaintReason
    {
        Harassment = 0,           // Преследование/Оскорбление
        Spam = 1,                 // Спам
        AdultContent = 2,         // Взрослый контент
        HarmfulContent = 3,       // Вредоносный контент
        MedicalFalseInfo = 4,     // Медицинская дезинформация
        Impersonation = 5,        // Выдача себя за другого
        PrivacyViolation = 6,     // Нарушение приватности
        CopyrightViolation = 7,   // Нарушение авторских прав
        Other = 8                 // Другое
    }

    /// <summary>
    /// Жалоба на контент
    /// Система должна соблюдать приватность - администратор видит ТОЛЬКО объект жалобы
    /// </summary>
    public class Complaint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Кто отправил жалобу (обязателен)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SenderUserId { get; set; } = null!;

        [ForeignKey(nameof(SenderUserId))]
        public virtual ApplicationUser? SenderUser { get; set; }

        /// <summary>
        /// На кого/что жалоба (обязателен)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TargetUserId { get; set; } = null!;

        [ForeignKey(nameof(TargetUserId))]
        public virtual ApplicationUser? TargetUser { get; set; }

        /// <summary>
        /// ID сообщения чата (nullable)
        /// </summary>
        [StringLength(36)]
        public string? MessageId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual ChatMessage? Message { get; set; }

        /// <summary>
        /// ID поста форума (nullable)
        /// </summary>
        public int? PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual ForumPost? Post { get; set; }

        /// <summary>
        /// Причина жалобы (выбор из enum)
        /// </summary>
        [Required]
        public ComplaintReason Reason { get; set; }

        /// <summary>
        /// Дополнительный текст причины
        /// </summary>
        [StringLength(1000)]
        public string? ReasonText { get; set; }

        /// <summary>
        /// Дата создания жалобы
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Статус жалобы (New, Reviewed, Resolved, Dismissed)
        /// </summary>
        [Required]
        public ComplaintStatus Status { get; set; } = ComplaintStatus.New;

        /// <summary>
        /// Кто рассматривал жалобу
        /// </summary>
        [StringLength(100)]
        public string? ReviewedByAdminId { get; set; }

        [ForeignKey(nameof(ReviewedByAdminId))]
        public virtual ApplicationUser? ReviewedByAdmin { get; set; }

        /// <summary>
        /// Дата рассмотрения
        /// </summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>
        /// Комментарий администратора
        /// </summary>
        [StringLength(1000)]
        public string? AdminComment { get; set; }

        /// <summary>
        /// Был ли применен бан
        /// </summary>
        public bool IsBanned { get; set; } = false;

        /// <summary>
        /// Был ли выдан предупреж
        /// </summary>
        public bool IsWarned { get; set; } = false;

        /// <summary>
        /// Дата обновления последнего статуса
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
