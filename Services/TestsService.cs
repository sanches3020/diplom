using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Tests;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Tests;

namespace Sofia.Web.Services;

public class TestsService : ITestsService
{
    private readonly SofiaDbContext _context;

    public TestsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetPsychologistIdAsync(int userId)
    {
        var psy = await _context.Psychologists.FirstOrDefaultAsync(p => p.UserId == userId);
        return psy?.Id;
    }

    public async Task<PsychologistTestsListViewModel?> GetTestsAsync(int psychologistUserId)
    {
        var psy = await _context.Psychologists.FirstOrDefaultAsync(p => p.UserId == psychologistUserId);
        if (psy == null) return null;

        var tests = await _context.Tests
            .Where(t => t.CreatedByPsychologistId == psy.Id)
            .OrderByDescending(t => t.Id)
            .ToListAsync();

        return new PsychologistTestsListViewModel
        {
            Tests = tests
        };
    }

    public async Task<(bool Success, string Message)> CreateTestAsync(int psychologistUserId, CreateTestRequest request)
    {
        var psy = await _context.Psychologists.FirstOrDefaultAsync(p => p.UserId == psychologistUserId);
        if (psy == null)
            return (false, "Психолог не найден");

        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Название обязательно");

        var test = new Test
        {
            Name = request.Name,
            Description = request.Description,
            Type = TestType.Custom,
            CreatedByPsychologistId = psy.Id
        };

        _context.Tests.Add(test);
        await _context.SaveChangesAsync();

        foreach (var q in request.Questions)
        {
            var question = new Question
            {
                TestId = test.Id,
                Text = q.Text,
                Type = AnswerType.SingleChoice
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            var answers = q.Answers
                .Select((a, i) => new Answer
                {
                    QuestionId = question.Id,
                    Text = a.Text,
                    Value = a.Value,
                    Order = i
                })
                .ToList();

            _context.Answers.AddRange(answers);
            await _context.SaveChangesAsync();
        }

        return (true, "Тест создан");
    }

    public async Task<(bool Success, string Message)> DeleteTestAsync(int psychologistUserId, int testId)
    {
        var psy = await _context.Psychologists.FirstOrDefaultAsync(p => p.UserId == psychologistUserId);
        if (psy == null)
            return (false, "Психолог не найден");

        var test = await _context.Tests
            .FirstOrDefaultAsync(t => t.Id == testId && t.CreatedByPsychologistId == psy.Id);

        if (test == null)
            return (false, "Тест не найден");

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();

        return (true, "Тест удалён");
    }
}
