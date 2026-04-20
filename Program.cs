using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Hubs;
using Sofia.Web.Models;
using Sofia.Web.Services;
using Sofia.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// MVC + антифорджери
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// ----------------------------
// База данных
// ----------------------------
builder.Services.AddDbContext<SofiaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------
// Identity
// ----------------------------
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<SofiaDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;

    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// ----------------------------
// CORS (SignalR требует AllowCredentials)
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:5001",
                "https://localhost:7135"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ----------------------------
// Сессии (ВАЖНО)
// ----------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// ----------------------------
// DI: сервисы приложения
// ----------------------------
builder.Services.AddScoped<IUserTestService, UserTestService>();
builder.Services.AddScoped<IPsychologistService, PsychologistService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IReviewsService, ReviewsService>();
builder.Services.AddScoped<IAppointmentsService, AppointmentsService>();
builder.Services.AddScoped<IClientResultsService, ClientResultsService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IPsychologistProfileService, PsychologistProfileService>();
builder.Services.AddScoped<IClientAnalyticsService, ClientAnalyticsService>();
builder.Services.AddScoped<IHomeService, HomeService>();

// Форум
builder.Services.AddScoped<IForumService, ForumService>();

// Медиа
builder.Services.AddScoped<IFileService, FileService>();

// Чат
builder.Services.AddSingleton<ChatStorage>();
builder.Services.AddScoped<IChatService, ChatService>();

// Прочие сервисы приложения
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<IGoalsService, GoalsService>();
builder.Services.AddScoped<ICompanionService, CompanionService>();

builder.Services.AddSignalR();

var app = builder.Build();

// ----------------------------
// Middleware
// ----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("Default");

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// СЕССИИ — ДОЛЖНЫ БЫТЬ ЗДЕСЬ
// ----------------------------
app.UseSession();

// ----------------------------
// Миграции + сидирование
// ----------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SofiaDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    context.Database.Migrate();

    await DatabaseSeeder.SeedAsync(context);

    const string adminRoleName = "admin";
    const string adminEmail = "admin@sofia.local";
    const string adminPassword = "Admin123!";

    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new ApplicationRole(adminRoleName));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Администратор системы",
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
        }
    }
}

// ----------------------------
// SignalR: Чат
// ----------------------------
app.MapHub<ChatHub>("/chatHub");

// ----------------------------
// MVC маршруты
// ----------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
