using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services;

public class PsychologistService : IPsychologistService
{
    private readonly SofiaDbContext _context;

    public PsychologistService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetPsychologistIdForUserAsync(int userId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return psychologist?.Id;
    }

    public async Task<PsychologistIndexViewModel> GetIndexDataAsync(int userId)
    {
        var psychologists = await _context.Psychologists
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId && n.ShareWithPsychologist)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        return new PsychologistIndexViewModel
        {
            Psychologists = psychologists,
            RecentNotes = recentNotes
        };
    }
}
