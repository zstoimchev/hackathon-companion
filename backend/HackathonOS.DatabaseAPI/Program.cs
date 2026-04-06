using HackathonOS.Domain.Configurations;
using HackathonOS.Infrastructure;
using HackathonOS.Infrastructure.UserPersistence;
using HackathonOS.Middleware;
using HackathonOS.Middleware.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// ─── Repositories ────────────────────────────────────────────────────────────
builder.Services.AddTransient<IUserRepository, UserRepositorySql>();

// ─── Application Services ────────────────────────────────────────────────────
builder.Services.AddTransient<ISharedDatabaseUtils, SharedDatabaseUtils>();

// ─── Configuration ───────────────────────────────────────────────────────────
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

// ─── API ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();

var app = builder.Build();

// ─── Middleware ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment()) app.UseSwaggerWithAuth();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();