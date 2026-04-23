using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services;

public class PsychologistProfileService : IPsychologistProfileService
{
    private readonly SofiaDbContext _context;

    public PsychologistProfileService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<PsychologistProfileViewModel?> GetProfileAsync(int psychologistId)
    {
        var psychologist = await _context.Psychologists
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == psychologistId && p.IsActive);

        if (psychologist == null)
            return null;

        // Загружаем отзывы
        var reviews = await _context.PsychologistReviews
            .AsNoTracking()
            .Where(r => r.PsychologistId == psychologistId &&
                        r.IsApproved &&
                        r.IsVisible)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToListAsync();

        var reviewVMs = new List<ReviewViewModel>();

        foreach (var review in reviews)
        {
            ApplicationUser? user = null;

            if (!string.IsNullOrEmpty(review.UserId))
            {
                user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == review.UserId);
            }

            reviewVMs.Add(new ReviewViewModel
            {
                Review = review,
                User = user
            });
        }

        // Доступные слоты
        var availableSlots = await _context.PsychologistTimeSlots
            .AsNoTracking()
            .Where(t => t.PsychologistId == psychologistId &&
                        t.IsAvailable &&
                        !t.IsBooked)
            .OrderBy(t => t.Date)
            .ToListAsync();

        availableSlots = availableSlots
            .OrderBy(t => t.Date)
            .ThenBy(t => t.StartTime)
            .ToList();

        return new PsychologistProfileViewModel
        {
            Psychologist = psychologist,
            Reviews = reviewVMs,
            AvailableSlots = availableSlots
        };
    }
}
