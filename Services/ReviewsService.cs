using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services;

public class ReviewsService : IReviewsService
{
    private readonly SofiaDbContext _context;

    public ReviewsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<PsychologistReviewsViewModel?> GetReviewsAsync(int psychologistUserId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return null;

        var reviews = await _context.PsychologistReviews
            .Where(r => r.PsychologistId == psychologist.Id)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return new PsychologistReviewsViewModel
        {
            Psychologist = psychologist,
            Reviews = reviews
        };
    }

    public async Task<(bool Success, string Message)> ApproveReviewAsync(int psychologistUserId, int reviewId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден");

        var review = await _context.PsychologistReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.PsychologistId == psychologist.Id);

        if (review == null)
            return (false, "Отзыв не найден");

        review.IsApproved = true;
        review.IsVisible = true;
        review.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Отзыв одобрен");
    }

    public async Task<(bool Success, string Message)> RejectReviewAsync(int psychologistUserId, int reviewId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден");

        var review = await _context.PsychologistReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.PsychologistId == psychologist.Id);

        if (review == null)
            return (false, "Отзыв не найден");

        review.IsApproved = false;
        review.IsVisible = false;
        review.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Отзыв отклонён");
    }

    public async Task<(bool Success, string Message)> DeleteReviewAsync(int psychologistUserId, int reviewId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден");

        var review = await _context.PsychologistReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.PsychologistId == psychologist.Id);

        if (review == null)
            return (false, "Отзыв не найден");

        _context.PsychologistReviews.Remove(review);
        await _context.SaveChangesAsync();

        return (true, "Отзыв удалён");
    }
}
