using Sofia.Web.DTO.UserTest;
using Sofia.Web.ViewModels.UserTest;

namespace Sofia.Web.Services.Interfaces;

public interface IUserTestService
{
    Task<UserTestListViewModel> GetTestsAsync();
    Task<UserTestAnalyticsViewModel> GetAnalyticsAsync();
    Task<object> GetAnalyticsDataAsync(int userId, UserTestAnalyticsRequest request);

    Task<UserTestTakeViewModel?> GetTestForTakingAsync(int id);
    Task<UserTestHistoryViewModel?> GetHistoryAsync(int userId, int testId);

    Task<UserTestSubmitResult> SubmitAsync(int userId, int testId, IFormCollection form);
    Task<UserTestResultViewModel?> GetResultAsync(int userId, int resultId);
}
