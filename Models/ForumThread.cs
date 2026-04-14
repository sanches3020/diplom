using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class ForumThread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Заголовок темы
        [Required]
        [StringLength(300)]
        public string Title { get; set; } = null!;

        // Дата создания
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Автор темы
        [Required]
        [StringLength(100)]
        public string AuthorId { get; set; } = null!;

        [ForeignKey(nameof(AuthorId))]
        public virtual ApplicationUser? Author { get; set; }

        // Категория
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual ForumCategory Category { get; set; } = null!;

        // Посты внутри темы
        public virtual ICollection<ForumPost> Posts { get; set; } = new List<ForumPost>();
    }
}
