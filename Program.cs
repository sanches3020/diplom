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

app.Run();