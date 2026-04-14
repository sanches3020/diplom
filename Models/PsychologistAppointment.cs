using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class PsychologistAppointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Психолог (обязательный)
        [Required]
        public int PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist Psychologist { get; set; } = null!;

        // Пользователь (обязательный)
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        // Дата и время записи
        [Required]
        public DateTime AppointmentDate { get; set; }

        // Заметки пользователя
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Статус записи (ENUM)
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        // Когда создана запись
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
