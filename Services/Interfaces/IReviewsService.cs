using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces;

public interface IReviewsService
{
    Task<PsychologistReviewsViewModel?> GetReviewsAsync(int psychologistUserId);
    Task<(bool Success, string Message)> ApproveReviewAsync(int psychologistUserId, int reviewId);
    Task<(bool Success, string Message)> RejectReviewAsync(int psychologistUserId, int reviewId);
    Task<(bool Success, string Message)> DeleteReviewAsync(int psychologistUserId, int reviewId);
}
