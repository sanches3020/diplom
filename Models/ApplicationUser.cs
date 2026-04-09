using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class ApplicationUser : IdentityUser
{

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsBlocked { get; set; } = false;


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string UserType { get; set; } = "user";

    public int? PsychologistProfileId { get; set; }

    [ForeignKey(nameof(PsychologistProfileId))]
    public virtual PsychologistProfile? PsychologistProfile { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public virtual ICollection<PsychologistAppointment> Appointments { get; set; } = new List<PsychologistAppointment>();
}
