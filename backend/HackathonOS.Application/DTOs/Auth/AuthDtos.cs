namespace HackathonOS.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Name, string Password, string Role);

public record AuthResponse(string Token, Guid UserId, string Email, string Name, string Role);
