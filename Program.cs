using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services;
using Sofia.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// 1. MVC + AntiForgery
// ----------------------------
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        // Автоматическая защита всех POST-запросов
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

// ----------------------------
// 2. PostgreSQL + DbContext
// ----------------------------
builder.Services.AddDbContext<SofiaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------
// 3. Identity (User + Roles)
// ----------------------------
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SofiaDbContext>()
.AddDefaultTokenProviders();

// ----------------------------
// 4. Cookie Authentication
// ----------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

// ----------------------------
// 5. DI: сервисы приложения
// ----------------------------
// (здесь ты регистрируешь свои сервисы)
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

var app = builder.Build();

// ----------------------------
// 6. Middleware
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ВАЖНО: порядок — сначала Authentication, потом Authorization
app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// 7. Миграции + сидирование
// ----------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SofiaDbContext>();

    // Применяем миграции (вместо EnsureCreated)
    context.Database.Migrate();

    // Сидирование вынесено в отдельный класс
    await DatabaseSeeder.SeedAsync(context);
}

// ----------------------------
// 8. Маршрутизация
// ----------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
