using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologists;

public class ClientDataViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<Note> Notes { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
    public List<EmotionEntry> Emotions { get; set; } = new();
    public List<PsychologistAppointment> RecentAppointments { get; set; } = new();
}
