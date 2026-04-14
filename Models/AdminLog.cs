using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class AdminLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AdminId { get; set; } = null!;

        [ForeignKey(nameof(AdminId))]
        public ApplicationUser Admin { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = null!;

        [StringLength(100)]
        public string? TargetUserId { get; set; }

        [StringLength(1000)]
        public string? Details { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(300)]
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
