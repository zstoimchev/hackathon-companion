using HackathonOS.Application.Interfaces;
using HackathonOS.Application.Mappings;
using HackathonOS.Application.Services;
using HackathonOS.Infrastructure.UserPersistence;
using HackathonOS.Middleware.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// ─── Repositories ────────────────────────────────────────────────────────────
builder.Services.AddTransient<IUserRepository, UserRepositoryClient>();

// ─── Application Services ────────────────────────────────────────────────────
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
builder.Services.AddTransient<IHashingService, HashingService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITeamService, TeamService>();

// ─── JWT Authentication ───────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                      ?? ["http://localhost:3000"];
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ─── API ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuth();

var app = builder.Build();

// ─── Middleware ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment()) app.UseSwaggerWithAuth();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseJwtAuthentication();
app.MapControllers();

app.Run();

internal abstract partial class Program
{
}