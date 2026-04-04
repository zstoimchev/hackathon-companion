using HackathonOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController(ITeamService teamService) : ControllerBase
{
    private readonly ITeamService _teamService = teamService;

    // /// <summary>Get all teams for an event.</summary>
    // [HttpGet("by-event/{eventId:guid}")]
    // [ProducesResponseType(typeof(IEnumerable<TeamResponse>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken ct)
    // {
    //     try { return Ok(await _teamService.GetByEventAsync(eventId, ct)); }
    //     catch (KeyNotFoundException) { return NotFound(); }
    // }
    //
    // /// <summary>Get team by ID.</summary>
    // [HttpGet("{id:guid}")]
    // [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    // {
    //     try { return Ok(await _teamService.GetByIdAsync(id, ct)); }
    //     catch (KeyNotFoundException) { return NotFound(); }
    // }
    //
    // /// <summary>Register a new team. Requires Admin role.</summary>
    // [HttpPost]
    // [Authorize(Roles = "Admin")]
    // [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status201Created)]
    // public async Task<IActionResult> Create([FromBody] CreateTeamRequest request, CancellationToken ct)
    // {
    //     try
    //     {
    //         var result = await _teams.CreateAsync(request, ct);
    //         return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    //     }
    //     catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    // }
    //
    // /// <summary>Update a team. Requires Admin role.</summary>
    // [HttpPut("{id:guid}")]
    // [Authorize(Roles = "Admin")]
    // [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    // public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamRequest request, CancellationToken ct)
    // {
    //     try { return Ok(await _teamService.UpdateAsync(id, request, ct)); }
    //     catch (KeyNotFoundException) { return NotFound(); }
    // }
    //
    // /// <summary>Delete a team. Requires Admin role.</summary>
    // [HttpDelete("{id:guid}")]
    // [Authorize(Roles = "Admin")]
    // [ProducesResponseType(StatusCodes.Status204NoContent)]
    // public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    // {
    //     try
    //     {
    //         await _teamService.DeleteAsync(id, ct);
    //         return NoContent();
    //     }
    //     catch (KeyNotFoundException) { return NotFound(); }
    // }
}
