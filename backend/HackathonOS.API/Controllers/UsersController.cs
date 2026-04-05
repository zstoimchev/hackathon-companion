using HackathonOS.Application.DTOs;
using HackathonOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : HackathonController
{
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUserAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken ct = default)
    {
        var result = await userService.CreateAsync(request, ct);
        return MapToActionResult(result);
    }
}