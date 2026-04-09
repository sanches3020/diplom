using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class PsychologistAppointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Пользователь
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        // Психолог
        public int PsychologistId { get; set; }
        public Psychologist Psychologist { get; set; } = null!;

        // Дата и время записи
        public DateTime AppointmentDate { get; set; }

        // Когда создана запись
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Статус записи (ENUM!)
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        // Заметки пользователя
        public string? Notes { get; set; }
    }
}
