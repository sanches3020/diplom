using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Полное имя пользователя
        [StringLength(100)]
        public string? FullName { get; set; }

        // Краткая биография / описание
        [StringLength(500)]
        public string? Bio { get; set; }

        // Активен ли пользователь
        public bool IsActive { get; set; } = true;

        // Заблокирован ли пользователь
        public bool IsBlocked { get; set; } = false;

        // Дата регистрации
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string UserType { get; set; } = "user";

        public int? PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist? Psychologist { get; set; }

        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public virtual ICollection<PsychologistAppointment> Appointments { get; set; } = new List<PsychologistAppointment>();
    }
}
