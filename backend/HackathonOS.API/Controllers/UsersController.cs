using HackathonOS.Application.DTOs;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : GdgController
{
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUserAsync(
        [FromBody] UserRequest request,
        CancellationToken ct = default)
    {
        var result = await userService.CreateUserAsync(request, ct);
        return MapToActionResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<Paginated<UserResponse>>> GetAllUsersAsync(
        [FromQuery] int pageNumber = 0,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await userService.GetAllUsersAsync(pageNumber, pageSize, ct);
        return MapToActionResult(result);
    }

    [HttpGet]
    [Route("{guid}")]
    public async Task<ActionResult<UserResponse>> GetUserDetailsAsync(
        string guid,
        CancellationToken ct = default)
    {
        var result = await userService.GetUserDetailsAsync(ParseGuid, ct);
        return MapToActionResult(result);
    }

    [HttpPut]
    public async Task<ActionResult<UserResponse>> UpdateUserAsync(
        string guid,
        [FromBody] UserRequest request,
        CancellationToken ct = default)
    {
        var result = await userService.UpdateUserAsync(ParseGuid, request, ct);
        return MapToActionResult(result);
    }

    [HttpDelete]
    public async Task<ActionResult<UserResponse>> DeleteUserAsync(
        string guid,
        CancellationToken ct = default)
    {
        var result = await userService.DeleteUserAsync(ParseGuid, ct);
        return MapToActionResult(result);
    }
}