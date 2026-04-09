using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IReviewsService
{
    Task<PsychologistReviewsViewModel?> GetReviewsAsync(string psychologistUserId);
    Task<(bool Success, string Message)> ApproveReviewAsync(string psychologistUserId, int reviewId);
    Task<(bool Success, string Message)> RejectReviewAsync(string psychologistUserId, int reviewId);
    Task<(bool Success, string Message)> DeleteReviewAsync(string psychologistUserId, int reviewId);
}
