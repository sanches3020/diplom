using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Goals;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Goals;

namespace Sofia.Web.Services;

public class GoalsService : IGoalsService
{
    private readonly SofiaDbContext _context;

    public GoalsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<GoalsListViewModel> GetGoalsAsync(int userId, string? sort)
    {
        var query = _context.Goals.Where(g => g.UserId == userId);

        // Seed sample goals if none exist
        if (!await query.AnyAsync())
        {
            var samples = new List<Goal>
            {
                new Goal { Title = "Улучшить сон", Description = "Режим сна", Type = GoalType.Wellness, Status = GoalStatus.Active, Progress = 0, CreatedAt = DateTime.Now, Date = DateTime.Today, TargetDate = DateTime.Today.AddMonths(1), UserId = userId },
                new Goal { Title = "Сделать презентацию", Description = "Подготовить слайды", Type = GoalType.Professional, Status = GoalStatus.Active, Progress = 0, CreatedAt = DateTime.Now.AddDays(-7), Date = DateTime.Today.AddDays(-7), TargetDate = DateTime.Today.AddDays(14), UserId = userId },
                new Goal { Title = "Пройти курс", Description = "Онлайн-курс", Type = GoalType.Professional, Status = GoalStatus.Active, Progress = 0, CreatedAt = DateTime.Now, Date = DateTime.Today, TargetDate = DateTime.Today.AddMonths(2), UserId = userId }
            };

            _context.Goals.AddRange(samples);
            await _context.SaveChangesAsync();

            query = _context.Goals.Where(g => g.UserId == userId);
        }

        // Sorting
        query = sort switch
        {
            "created_asc" => query.OrderBy(g => g.CreatedAt),
            "created_desc" => query.OrderByDescending(g => g.CreatedAt),
            "deadline_asc" => query.OrderBy(g => g.TargetDate ?? DateTime.MaxValue),
            "deadline_desc" => query.OrderByDescending(g => g.TargetDate ?? DateTime.MinValue),
            "progress_asc" => query.OrderBy(g => g.Progress),
            "progress_desc" => query.OrderByDescending(g => g.Progress),
            _ => query.OrderByDescending(g => g.Status == GoalStatus.Active)
                      .ThenByDescending(g => g.CreatedAt)
        };

        return new GoalsListViewModel
        {
            Goals = await query.ToListAsync(),
            CurrentSort = sort
        };
    }

    public async Task<Goal?> GetGoalAsync(int userId, int id)
    {
        return await _context.Goals
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CreateGoalAsync(int userId, CreateGoalRequest request)
    {
        var goal = new Goal
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Status = request.Status,
            Progress = request.Progress,
            Date = request.Date,
            TargetDate = request.TargetDate,
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateGoalAsync(int userId, UpdateGoalRequest request)
    {
        var goal = await GetGoalAsync(userId, request.Id);
        if (goal == null) return false;

        goal.Title = request.Title;
        goal.Description = request.Description;
        goal.Type = request.Type;
        goal.Status = request.Status;
        goal.Progress = request.Progress;
        goal.Date = request.Date;
        goal.TargetDate = request.TargetDate;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGoalAsync(int userId, int id)
    {
        var goal = await GetGoalAsync(userId, id);
        if (goal == null) return false;

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProgressAsync(int userId, int id, int progress)
    {
        var goal = await GetGoalAsync(userId, id);
        if (goal == null) return false;

        goal.Progress = Math.Clamp(progress, 0, 100);
        if (goal.Progress == 100)
            goal.Status = GoalStatus.Completed;

        await _context.SaveChangesAsync();
        return true;
    }
}
