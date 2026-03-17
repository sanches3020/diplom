using Sofia.Web.DTO.Goals;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Goals;

namespace Sofia.Web.Services.Interfaces;

public interface IGoalsService
{
    Task<GoalsListViewModel> GetGoalsAsync(int userId, string? sort);
    Task<Goal?> GetGoalAsync(int userId, int id);
    Task<bool> CreateGoalAsync(int userId, CreateGoalRequest request);
    Task<bool> UpdateGoalAsync(int userId, UpdateGoalRequest request);
    Task<bool> DeleteGoalAsync(int userId, int id);
    Task<bool> UpdateProgressAsync(int userId, int id, int progress);
}
