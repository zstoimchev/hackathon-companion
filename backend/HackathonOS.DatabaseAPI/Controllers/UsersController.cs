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
        var created = await userRepository.CreateAsync(user, ct);
        return created is null ? Conflict() : Ok(created);
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUsersAsync(
        [FromQuery] int pageNumber = 0, 
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetAllAsync(pageNumber, pageSize, ct);
        return Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUserAsync(
        int id,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<User>> GetUserAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<User>> UpdateUserAsync(
        int id, 
        User user,
        CancellationToken ct = default)
    {
        var updated = await userRepository.UpdateAsync(id, user, ct);
        return updated == null ? Conflict() : Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUserAsync(
        int id,
        CancellationToken ct = default)
    {
        var deleted = await userRepository.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}