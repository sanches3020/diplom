using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Home;

namespace Sofia.Web.Services;

public class HomeService : IHomeService
{
    private readonly SofiaDbContext _context;

    public HomeService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetPsychologistIdForUserAsync(int userId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return psychologist?.Id;
    }

    public async Task<HomeIndexViewModel> GetHomePageDataAsync()
    {
        var psychologists = await _context.Psychologists
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return new HomeIndexViewModel
        {
            Psychologists = psychologists
        };
    }
}
