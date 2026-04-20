using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class GoalAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Ссылка на цель
        /// </summary>
        [Required]
        public int GoalId { get; set; }

        [ForeignKey(nameof(GoalId))]
        public virtual Goal? Goal { get; set; }

        /// <summary>
        /// ID пользователя
        /// </summary>
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Что сделано (текст действия)
        /// </summary>
        [Required]
        public string ActionText { get; set; } = string.Empty;

        /// <summary>
        /// Какой результат получен (текст результата)
        /// </summary>
        [Required]
        public string ResultText { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания записи
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
