using System.Security.Claims;
using HackathonOS.Application.DTOs.MentorRequests;
using HackathonOS.Application.Services;
using HackathonOS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackathonOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MentorRequestsController : ControllerBase
{
    private readonly MentorRequestService _service;

    public MentorRequestsController(MentorRequestService service) => _service = service;

    /// <summary>Create a new mentor help request.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MentorRequestResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateMentorRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetByEvent), new { eventId = result.EventId }, result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    /// <summary>Get all mentor requests for an event.</summary>
    [HttpGet("by-event/{eventId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<MentorRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken ct)
        => Ok(await _service.GetByEventAsync(eventId, ct));

    /// <summary>Get only waiting requests for an event (mentor queue view).</summary>
    [HttpGet("queue/{eventId:guid}")]
    [Authorize(Roles = "Mentor,Admin")]
    [ProducesResponseType(typeof(IEnumerable<MentorRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQueue(Guid eventId, CancellationToken ct)
        => Ok(await _service.GetWaitingAsync(eventId, ct));

    /// <summary>Assign a mentor to a request. Mentor self-assigns or Admin assigns.</summary>
    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "Mentor,Admin")]
    [ProducesResponseType(typeof(MentorRequestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Assign(Guid id, CancellationToken ct)
    {
        var mentorId = GetCurrentUserId();
        try
        {
            var result = await _service.AssignMentorAsync(id, mentorId, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Update request status (InProgress, Done, Cancelled).</summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Mentor,Admin")]
    [ProducesResponseType(typeof(MentorRequestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(id, request.Status, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}

public record UpdateStatusRequest(MentorRequestStatus Status);
