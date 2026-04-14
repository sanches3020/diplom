using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class ChatParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Комната
        [Required]
        public int RoomId { get; set; }

        [ForeignKey(nameof(RoomId))]
        public virtual ChatRoom Room { get; set; } = null!;

        // Пользователь
        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        // Когда пользователь присоединился к комнате
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
