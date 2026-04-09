using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModel.ChoosePsychologist;

namespace Sofia.Web.Services;

public class ChoosePsychologistService : IChoosePsychologistService
{
    private readonly SofiaDbContext _context;

    public ChoosePsychologistService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<ChoosePsychologistViewModel> GetActivePsychologistsAsync()
    {
        var psychologists = await _context.Psychologists
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return new ChoosePsychologistViewModel
        {
            Psychologists = psychologists
        };
    }
}
