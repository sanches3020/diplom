var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddWeb(builder.Configuration);

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

// Pipeline
app.UseApplicationMiddleware();

app.MapApplicationEndpoints();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await Sofia.Web.Data.DbInitializer.InitializeAsync(scope.ServiceProvider);
}

app.Run();