using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services;
using Sofia.Web.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Singleton
        services.AddSingleton<ChatStorage>();

        // Основные сервисы
        services.AddScoped<IUserTestService, UserTestService>();
        services.AddScoped<IForumService, ForumService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<INotesService, NotesService>();
        services.AddScoped<IGoalsService, GoalsService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<ITestsService, TestsService>();
        services.AddScoped<IChoosePsychologistService, ChoosePsychologistService>();
        services.AddScoped<IPracticesService, PracticesService>();

        // Психологи и клиентская логика
        services.AddScoped<IPsychologistService, PsychologistService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IReviewsService, ReviewsService>();
        services.AddScoped<IAppointmentsService, AppointmentsService>();
        services.AddScoped<IClientResultsService, ClientResultsService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<IPsychologistProfileService, PsychologistProfileService>();
        services.AddScoped<IClientAnalyticsService, ClientAnalyticsService>();

        // Дополнительные сервисы
        services.AddScoped<ICompanionService, CompanionService>();
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IFileService, FileService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<SofiaDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<SofiaDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        services.AddSignalR();

        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromHours(12);
        });

        services.AddCors(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

                if (origins is not null && origins.Length > 0)
                {
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
            });
        });

        return services;
    }
}
