using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Psychologist;
using System.Text;

namespace Sofia.Web.Services;

public class ClientResultsService : IClientResultsService
{
    private readonly SofiaDbContext _context;

    public ClientResultsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<ClientResultsViewModel?> GetClientResultsAsync(int psychologistUserId, int clientId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return null;

        // Проверяем, что клиент действительно связан с психологом
        var hasRelation = await _context.PsychologistAppointments
            .AnyAsync(a => a.PsychologistId == psychologist.Id && a.UserId == clientId);

        if (!hasRelation)
            return null;

        var client = await _context.Users.FirstOrDefaultAsync(u => u.Id == clientId);
        if (client == null)
            return null;

        var results = await _context.TestResults
            .Where(r => r.UserId == clientId)
            .Include(r => r.Test)
            .OrderByDescending(r => r.TakenAt)
            .ToListAsync();

        return new ClientResultsViewModel
        {
            Client = client,
            Psychologist = psychologist,
            Results = results
        };
    }

    public async Task<(bool Success, string Message, byte[]? FileBytes, string? FileName)> GetClientResultsCsvAsync(int psychologistUserId, int clientId)
    {
        var vm = await GetClientResultsAsync(psychologistUserId, clientId);
        if (vm == null)
            return (false, "Доступ запрещён или данные не найдены", null, null);

        var sb = new StringBuilder();
        sb.AppendLine("Дата,Тест,Баллы,Уровень,Интерпретация");

        foreach (var r in vm.Results)
        {
            var line =
                $"{r.TakenAt:yyyy-MM-dd HH:mm}," +
                $"\"{r.Test?.Name}\"," +
                $"{r.Score}," +
                $"\"{r.Level}\"," +
                $"\"{(r.Interpretation ?? string.Empty).Replace("\"", "'")}\"";

            sb.AppendLine(line);
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var fileName = $"client_{clientId}_results_{DateTime.Now:yyyyMMdd}.csv";

        return (true, "CSV сформирован", bytes, fileName);
    }
}
