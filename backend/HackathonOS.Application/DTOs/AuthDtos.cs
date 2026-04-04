namespace HackathonOS.Application.DTOs;

public record AuthRequest(
    string Email,
    string Password);

public record AuthResponse(
    string Token,
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string Role);