using Sofia.Web.DTO.Goals;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Goals;

namespace Sofia.Web.Services.Interfaces;

public interface IGoalsService
{
    Task<GoalsListViewModel> GetGoalsAsync(string userId, string? sort);
    Task<Goal?> GetGoalAsync(string userId, int id);
    Task<bool> CreateGoalAsync(string userId, CreateGoalRequest request);
    Task<bool> UpdateGoalAsync(string userId, UpdateGoalRequest request);
    Task<bool> DeleteGoalAsync(string userId, int id);
    Task<bool> UpdateProgressAsync(string userId, int id, int progress);
}
