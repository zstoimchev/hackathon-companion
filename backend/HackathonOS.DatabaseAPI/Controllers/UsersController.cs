using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.UserPersistence;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.DatabaseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<User>> CreateUserAsync(
        User user,
        CancellationToken ct = default)
    {
        var created = await userRepository.CreateUserAsync(user, ct);
        return created is null ? Conflict() : Ok(created);
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUsersAsync(
        [FromQuery] int pageNumber = 0, 
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetAllUsersAsync(pageNumber, pageSize, ct);
        return Ok(user);
    }

    [HttpGet("{guid:guid}")]
    public async Task<ActionResult<User>> GetUserByGuidAsync(
        Guid guid,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetUserDetailsAsync(guid, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<User>> GetUserByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetUserDetailsAsync(email, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{guid:guid}")]
    public async Task<ActionResult<User>> UpdateUserAsync(
        Guid guid, 
        User user,
        CancellationToken ct = default)
    {
        var updated = await userRepository.UpdateUserAsync(guid, user, ct);
        return updated == null ? Conflict() : Ok(user);
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> DeleteUserAsync(
        Guid guid,
        CancellationToken ct = default)
    {
        var deleted = await userRepository.DeleteUserAsync(guid, ct);
        return deleted ? NoContent() : NotFound();
    }
}