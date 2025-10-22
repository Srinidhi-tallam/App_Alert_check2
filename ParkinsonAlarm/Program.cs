using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkinsonAlarm.Data;
using ParkinsonAlarm.Hubs;
using ParkinsonAlarm.Services;
using ParkinsonAlarm.Services.Interfaces;
using Microsoft.AspNetCore.Routing;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Services
// Configure JSON to ignore reference cycles so EF navigation properties don't cause 500 during serialization
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
//var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
//builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// DB provider - prefer SQL Server when a connection string is configured, otherwise fall back to InMemory
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connection))
{
    // Use SQL Server (ensure appsettings.json contains DefaultConnection)
    builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(connection));
}
else if (builder.Environment.IsDevelopment())
{
    // Development fallback to in-memory database (no external DB required)
    builder.Services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("ParkinsonAlarmDev"));
}
else
{
    // Production without a connection string - fallback to an in-memory DB to avoid startup failure.
    builder.Services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("ParkinsonAlarmFallback"));
}

// SignalR + DI
builder.Services.AddSignalR();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAlarmService, AlarmService>();

var app = builder.Build();

// simple request logging
app.Use(async (ctx, next) =>
{
    var logger = ctx.RequestServices.GetService<ILogger<Program>>();
    logger?.LogInformation("Incoming request {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
    await next();
});

// Safe DB initialization: attempt to connect when using SQL Server, ensure created when using InMemory
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dbLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var providerName = db.Database.ProviderName ?? string.Empty;
        if (providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            // Ensure created and optionally seed in-memory DB
            db.Database.EnsureCreated();
            dbLogger.LogInformation("Using InMemory database; ensured created. Seed if necessary.");
            // If you want to seed data automatically in dev, uncomment:
            // SeedData.Initialize(db);
        }
        else
        {
            // Relational providers: attempt to connect, but don't crash app on failure
            if (await db.Database.CanConnectAsync())
            {
                dbLogger.LogInformation("Database reachable. Provider: {Provider}.", providerName);
                // To apply migrations automatically uncomment the following line:
                // await db.Database.MigrateAsync();
            }
            else
            {
                dbLogger.LogWarning("Database not reachable. Skipping migrations/seeding so app can start. Fix connection and run migrations manually.");
            }
        }
    }
    catch (Exception ex)
    {
        dbLogger.LogError(ex, "DB initialization check failed. Skipping migrations so the app can start.");
    }
}

// Middleware - ensure Swagger is available in Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseCors();
app.UseCors("AllowAngularDev");
app.MapControllers();
app.MapHub<AlarmHub>("/alarmsHub");

// log registered endpoints so we can confirm the route exists
var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var startupLogger = loggerFactory.CreateLogger<Program>();
foreach (var ep in endpointDataSource.Endpoints)
{
    startupLogger.LogInformation("Registered endpoint: {Endpoint}", ep.DisplayName ?? ep.ToString());
}

app.Run();