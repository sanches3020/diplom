using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class Psychologist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // -----------------------------
        // Основная информация
        // -----------------------------
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // -----------------------------
        // Связь с пользователем (Identity)
        // -----------------------------
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        // -----------------------------
        // Профессиональная информация
        // -----------------------------
        [StringLength(500)]
        public string? Specialization { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Education { get; set; }

        [StringLength(200)]
        public string? Experience { get; set; }

        [StringLength(100)]
        public string? Languages { get; set; }

        [StringLength(200)]
        public string? Methods { get; set; }

        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal? PricePerHour { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(100)]
        public string? ContactEmail { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // -----------------------------
        // Навигационные свойства
        // -----------------------------
        public virtual ICollection<PsychologistReview> Reviews { get; set; } = new List<PsychologistReview>();
        public virtual ICollection<PsychologistAppointment> Appointments { get; set; } = new List<PsychologistAppointment>();
        public virtual ICollection<PsychologistSchedule> Schedules { get; set; } = new List<PsychologistSchedule>();
        public virtual ICollection<PsychologistTimeSlot> TimeSlots { get; set; } = new List<PsychologistTimeSlot>();
    }
}
