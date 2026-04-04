using HackathonOS.Application.DTOs;
using HackathonOS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>Login with email and password.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request, CancellationToken ct)
    {
        try
        {
            var result = await authService.LoginAsync(request, ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
    }
}