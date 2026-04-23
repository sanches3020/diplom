using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.UserTest;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.UserTest;

namespace Sofia.Web.Services;

public class UserTestService : IUserTestService
{
    private readonly SofiaDbContext _context;

    public UserTestService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<UserTestListViewModel> GetTestsAsync()
    {
        var tests = await _context.Tests
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        return new UserTestListViewModel { Tests = tests };
    }

    public async Task<UserTestAnalyticsViewModel> GetAnalyticsAsync()
    {
        var tests = await _context.Tests
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        return new UserTestAnalyticsViewModel { Tests = tests };
    }

    public async Task<object> GetAnalyticsDataAsync(string userId, UserTestAnalyticsRequest req)
    {
        var results = await _context.TestResults
            .AsNoTracking()
            .Where(r => r.UserId == userId &&
                        r.TestId == req.TestId &&
                        r.TakenAt >= req.From &&
                        r.TakenAt <= req.To)
            .OrderBy(r => r.TakenAt)
            .Select(r => new
            {
                date = r.TakenAt,
                score = r.Score,
                level = r.Level,
                interpretation = r.Interpretation
            })
            .ToListAsync();

        if (!results.Any())
            return new { success = true, data = Array.Empty<object>() };

        var avg = results.Average(r => r.score);
        var firstHalf = results.Take(results.Count / 2).DefaultIfEmpty(results.First()).Average(r => r.score);
        var secondHalf = results.Skip(results.Count / 2).DefaultIfEmpty(results.Last()).Average(r => r.score);

        var trend = secondHalf > firstHalf ? "Рост"
                   : secondHalf < firstHalf ? "Падение"
                   : "Стабильно";

        return new { success = true, data = results, avg, trend };
    }

    public async Task<UserTestTakeViewModel?> GetTestForTakingAsync(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Questions.OrderBy(q => q.Id))
                .ThenInclude(q => q.Answers.OrderBy(a => a.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return test == null ? null : new UserTestTakeViewModel { Test = test };
    }

    public async Task<UserTestHistoryViewModel?> GetHistoryAsync(string userId, int testId)
    {
        var test = await _context.Tests
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null) return null;

        var results = await _context.TestResults
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.TestId == testId)
            .OrderByDescending(r => r.TakenAt)
            .ToListAsync();

        return new UserTestHistoryViewModel { Test = test, Results = results };
    }

    public async Task<UserTestSubmitResult> SubmitAsync(string userId, int testId, IFormCollection form)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            return new UserTestSubmitResult { Success = false, Message = "Тест не найден" };

        var answers = new List<UserAnswer>();

        foreach (var q in test.Questions)
        {
            var key = $"q_{q.Id}";
            if (!form.ContainsKey(key)) continue;

            var value = form[key].ToString();
            if (string.IsNullOrEmpty(value)) continue;

            if (q.Type == AnswerType.Text)
            {
                answers.Add(new UserAnswer
                {
                    UserId = userId,
                    QuestionId = q.Id,
                    TextAnswer = value,
                    CreatedAt = DateTime.UtcNow
                });
                continue;
            }

            foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part, out var ansId))
                {
                    answers.Add(new UserAnswer
                    {
                        UserId = userId,
                        QuestionId = q.Id,
                        AnswerId = ansId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        if (answers.Any())
        {
            _context.UserAnswers.AddRange(answers);
            await _context.SaveChangesAsync();
        }

        var selectedIds = answers
            .Where(a => a.AnswerId.HasValue)
            .Select(a => a.AnswerId!.Value)
            .ToList();

        var selectedAnswers = await _context.Answers
            .Where(a => selectedIds.Contains(a.Id))
            .ToListAsync();

        var score = selectedAnswers.Sum(a => a.Value);

        var maxScore = test.Questions.Sum(q =>
            q.Answers.Any() ? q.Answers.Max(a => a.Value) : 0);

        var percent = maxScore > 0 ? (double)score / maxScore * 100 : 0;

        var rule = await _context.TestInterpretations
            .Where(ti => ti.TestId == test.Id &&
                         percent >= ti.MinPercent &&
                         percent <= ti.MaxPercent)
            .FirstOrDefaultAsync();

        var level = rule?.Level ??
                    (percent < 33 ? "Низкий" :
                     percent < 66 ? "Средний" : "Высокий");

        var interpretation = rule?.InterpretationText ??
                            $"{level} уровень";

        var result = new TestResult
        {
            UserId = userId,
            TestId = test.Id,
            TakenAt = DateTime.UtcNow,
            Score = score,
            Level = level,
            Interpretation = interpretation
        };

        _context.TestResults.Add(result);
        await _context.SaveChangesAsync();

        return new UserTestSubmitResult
        {
            Success = true,
            RedirectUrl = $"/tests/result/{result.Id}"
        };
    }

    public async Task<UserTestResultViewModel?> GetResultAsync(string userId, int resultId)
    {
        var result = await _context.TestResults
            .Include(r => r.Test)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == resultId && r.UserId == userId);

        if (result == null) return null;

        var history = await _context.TestResults
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.TestId == result.TestId)
            .OrderBy(r => r.TakenAt)
            .Select(r => new { r.TakenAt, r.Score })
            .ToListAsync<object>();

        return new UserTestResultViewModel { Result = result, History = history };
    }
}
