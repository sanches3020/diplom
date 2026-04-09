using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Psychologist;
using Sofia.Web.ViewModels.Psychologists;

namespace Sofia.Web.Services;

public class ClientAnalyticsService : IClientAnalyticsService
{
    private readonly SofiaDbContext _context;

    public ClientAnalyticsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<PsychologistDashboardViewModel?> GetDashboardAsync(string psychologistUserId)
    {
        // Находим психолога по UserId (string)
        var psychologist = await _context.Psychologists
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return null;

        // Все записи на консультации
        var appointments = await _context.PsychologistAppointments
            .Where(a => a.PsychologistId == psychologist.Id)
            .Include(a => a.User)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        // Уникальные клиенты
        var clients = appointments
            .Where(a => a.User != null)
            .Select(a => a.User!)
            .Distinct()
            .ToList();

        var clientDataList = new List<ClientDataViewModel>();

        foreach (var client in clients)
        {
            var notes = await _context.Notes
                .Where(n => n.UserId == client.Id && n.ShareWithPsychologist)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            var goals = await _context.Goals
                .Where(g => g.UserId == client.Id)
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
                .ToListAsync();

            var emotions = await _context.EmotionEntries
                .Where(e => e.UserId == client.Id)
                .OrderByDescending(e => e.Date)
                .Take(10)
                .ToListAsync();

            var recentAppointments = await _context.PsychologistAppointments
                .Where(a => a.PsychologistId == psychologist.Id && a.UserId == client.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(3)
                .ToListAsync();

            clientDataList.Add(new ClientDataViewModel
            {
                User = client,
                Notes = notes,
                Goals = goals,
                Emotions = emotions,
                RecentAppointments = recentAppointments
            });
        }

        return new PsychologistDashboardViewModel
        {
            Psychologist = psychologist,
            Appointments = appointments,
            Clients = clientDataList
        };
    }
}
