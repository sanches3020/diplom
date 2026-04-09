using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class PsychologistProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // -----------------------------
    // Связь 1:1 с ApplicationUser
    // -----------------------------
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    // -----------------------------
    // Профессиональная информация
    // -----------------------------
    [StringLength(200)]
    public string? Title { get; set; } // например: "Клинический психолог"

    [StringLength(500)]
    public string? Specialization { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? Education { get; set; }

    [StringLength(200)]
    public string? Experience { get; set; }

    [StringLength(200)]
    public string? Methods { get; set; }

    [StringLength(200)]
    public string? Languages { get; set; }

    [StringLength(500)]
    public string? PhotoUrl { get; set; }

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
