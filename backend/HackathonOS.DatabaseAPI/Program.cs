using HackathonOS.Domain.Configurations;
using HackathonOS.Infrastructure;
using HackathonOS.Infrastructure.UserPersistence;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// ─── Configuration ───────────────────────────────────────────────────────────
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

// ─── Database ────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<ISharedDatabaseUtils, SharedDatabaseUtils>();

// ─── Repositories ────────────────────────────────────────────────────────────
builder.Services.AddTransient<IUserRepository, UserRepositorySQL>();

// ─── API ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hackathon Companion DatabaseAPI",
        Version = "v1",
        Description = "Fair, bias-corrected hackathon judging and mentor management platform (Data Access)"
    });
});

var app = builder.Build();

// ─── Middleware ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hackathon OS v1"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();