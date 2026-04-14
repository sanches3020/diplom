using System;
using System.ComponentModel.DataAnnotations;

namespace Sofia.Web.Models
{
    public class ChatMessage
    {
        // Уникальный идентификатор сообщения
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Комната (general, support, psychology, random)
        [Required]
        [StringLength(50)]
        public string Room { get; set; } = "general";

        // Пользователь
        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = null!;

        // Аватар-код (цвет + инициалы)
        [Required]
        [StringLength(20)]
        public string AvatarCode { get; set; } = null!;

        // Текст сообщения
        [Required]
        [StringLength(2000)]
        public string Text { get; set; } = null!;

        // Время отправки
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Системное сообщение (вход/выход)
        public bool IsSystem { get; set; } = false;
    }
}
