using Sofia.Web.DTO.UserTest;
using Sofia.Web.ViewModels.UserTest;

namespace Sofia.Web.Services.Interfaces;

public interface IUserTestService
{
    Task<UserTestListViewModel> GetTestsAsync();
    Task<UserTestAnalyticsViewModel> GetAnalyticsAsync();

    Task<object> GetAnalyticsDataAsync(string userId, UserTestAnalyticsRequest request);

    Task<UserTestTakeViewModel?> GetTestForTakingAsync(int id);

    Task<UserTestHistoryViewModel?> GetHistoryAsync(string userId, int testId);

    Task<UserTestSubmitResult> SubmitAsync(string userId, int testId, IFormCollection form);

    Task<UserTestResultViewModel?> GetResultAsync(string userId, int resultId);
}
