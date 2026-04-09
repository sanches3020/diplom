using Sofia.Web.DTO.Tests;
using Sofia.Web.ViewModels.Tests;

namespace Sofia.Web.Services.Interfaces;

public interface ITestsService
{
    Task<int?> GetPsychologistIdAsync(string userId);

    Task<PsychologistTestsListViewModel?> GetTestsAsync(string psychologistUserId);

    Task<(bool Success, string Message)> CreateTestAsync(string psychologistUserId, CreateTestRequest request);

    Task<(bool Success, string Message)> DeleteTestAsync(string psychologistUserId, int testId);
}
