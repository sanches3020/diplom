using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.PsychologistArea;
using System.Text;

namespace Sofia.Web.Services;

public class ClientResultsService : IClientResultsService
{
    private readonly SofiaDbContext _context;

    public ClientResultsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<ClientResultsViewModel?> GetClientResultsAsync(string psychologistUserId, string clientUserId)
    {
        // Находим психолога по UserId
        var psychologist = await _context.Psychologists
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return null;

        // Проверяем, что клиент действительно связан с психологом
        var hasRelation = await _context.PsychologistAppointments
            .AnyAsync(a => a.PsychologistId == psychologist.Id && a.UserId == clientUserId);

        if (!hasRelation)
            return null;

        // Находим клиента (Identity User)
        var client = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == clientUserId);

        if (client == null)
            return null;

        // Получаем результаты тестов
        var results = await _context.TestResults
            .Where(r => r.UserId == clientUserId)
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


    public async Task<ClientResultsCsvResponse> GetClientResultsCsvAsync(string psychologistUserId, string clientUserId)
    {
        var vm = await GetClientResultsAsync(psychologistUserId, clientUserId);
        if (vm == null)
            return new ClientResultsCsvResponse { Success = false, Message = "Доступ запрещён или данные не найдены" };

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
        var fileName = $"client_{clientUserId}_results_{DateTime.UtcNow:yyyyMMdd}.csv";

        return new ClientResultsCsvResponse { Success = true, Message = "CSV сформирован", FileBytes = bytes, FileName = fileName };
    }

    public Task<ClientResultsCsvResponse> GetClientResultsCsvAsync(string psychologistUserId, int clientId)
    {
        throw new NotImplementedException();
    }
}
