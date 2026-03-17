using Sofia.Web.DTO.Tests;
using Sofia.Web.ViewModels.Tests;

namespace Sofia.Web.Services.Interfaces;

public interface ITestsService
{
    Task<int?> GetPsychologistIdAsync(int userId);

    Task<PsychologistTestsListViewModel?> GetTestsAsync(int psychologistUserId);

    Task<(bool Success, string Message)> CreateTestAsync(int psychologistUserId, CreateTestRequest request);

    Task<(bool Success, string Message)> DeleteTestAsync(int psychologistUserId, int testId);
}
