using HackathonOS.Application.DTOs;

namespace HackathonOS.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(AuthRequest authRequest, CancellationToken ct = default);
    // Task<AuthResponse> RegisterAsync(AuthRequest authRequest, CancellationToken ct = default);
    Task LogoutAsync(string token, CancellationToken ct = default);
}