using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class ForumPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Текст поста
        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = null!;

        // Дата создания
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Автор поста
        [Required]
        [StringLength(100)]
        public string AuthorId { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public virtual ApplicationUser? Author { get; set; }

        // Тред, к которому относится пост
        [Required]
        public int ThreadId { get; set; }

        [ForeignKey(nameof(ThreadId))]
        public virtual ForumThread Thread { get; set; } = null!;

        // Лайки
        public virtual ICollection<ForumPostLike> Likes { get; set; } = new List<ForumPostLike>();

        // Медиафайл (опционально)
        public Guid? MediaFileId { get; set; }

        [ForeignKey(nameof(MediaFileId))]
        public virtual MediaFile? MediaFile { get; set; }
    }
}
