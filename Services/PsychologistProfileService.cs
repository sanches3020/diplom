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
            .FirstOrDefaultAsync(p => p.Id == psychologistId && p.IsActive);

        if (psychologist == null)
            return null;

        // Загружаем отзывы
        var reviews = await _context.PsychologistReviews
            .Where(r => r.PsychologistId == psychologistId && r.IsApproved && r.IsVisible)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToListAsync();

        var reviewVMs = new List<ReviewViewModel>();

        foreach (var review in reviews)
        {
            User? user = null;

            if (review.UserId.HasValue)
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == review.UserId.Value);
            }

            reviewVMs.Add(new ReviewViewModel
            {
                Review = review,
                User = user
            });
        }

        // Доступные слоты
        var availableSlots = await _context.PsychologistTimeSlots
            .Where(t => t.PsychologistId == psychologistId && t.IsAvailable && !t.IsBooked)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.StartTime)
            .ToListAsync();

        return new PsychologistProfileViewModel
        {
            Psychologist = psychologist,
            Reviews = reviewVMs,
            AvailableSlots = availableSlots
        };
    }
}
