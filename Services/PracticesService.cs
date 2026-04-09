using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Practices;

namespace Sofia.Web.Services;

public class PracticesService : IPracticesService
{
    private readonly SofiaDbContext _context;

    public PracticesService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<PracticesListViewModel> GetPracticesAsync(string? category, int? duration)
    {
        var query = _context.Practices
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(category) &&
            Enum.TryParse<PracticeCategory>(category, out var cat))
        {
            query = query.Where(p => p.Category == cat);
        }

        if (duration.HasValue)
        {
            query = query.Where(p => p.DurationMinutes <= duration.Value);
        }

        var practices = await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync();

        return new PracticesListViewModel
        {
            Practices = practices,
            Categories = Enum.GetValues<PracticeCategory>(),
            SelectedCategory = category,
            SelectedDuration = duration
        };
    }

    public async Task<Practice?> GetPracticeAsync(int id)
    {
        return await _context.Practices
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
