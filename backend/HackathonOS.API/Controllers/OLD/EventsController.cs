// using HackathonOS.Application.DTOs.Events;
// using HackathonOS.Application.Services;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace HackathonOS.Api.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// [Authorize]
// public class EventsController(EventService eventsService) : ControllerBase
// {
//     /// <summary>Get all events.</summary>
//     [HttpGet]
//     [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
//     public async Task<IActionResult> GetAll(CancellationToken ct)
//         => Ok(await eventsService.GetAllAsync(ct));
//
//     /// <summary>Get event by ID.</summary>
//     [HttpGet("{id:guid}")]
//     [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
//     {
//         try { return Ok(await eventsService.GetByIdAsync(id, ct)); }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     /// <summary>Create a new event. Requires Admin role.</summary>
//     [HttpPost]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
//     public async Task<IActionResult> Create([FromBody] CreateEventRequest request, CancellationToken ct)
//     {
//         var result = await eventsService.CreateAsync(request, ct);
//         return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
//     }
//
//     /// <summary>Update an event. Requires Admin role.</summary>
//     [HttpPut("{id:guid}")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request, CancellationToken ct)
//     {
//         try { return Ok(await eventsService.UpdateAsync(id, request, ct)); }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     /// <summary>Delete an event. Requires Admin role.</summary>
//     [HttpDelete("{id:guid}")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//     {
//         try
//         {
//             await eventsService.DeleteAsync(id, ct);
//             return NoContent();
//         }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     /// <summary>Add a judge to an event. Requires Admin role.</summary>
//     [HttpPost("{id:guid}/judges")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     public async Task<IActionResult> AddJudge(Guid id, [FromBody] AddJudgeRequest request, CancellationToken ct)
//     {
//         try
//         {
//             await eventsService.AddJudgeAsync(id, request, ct);
//             return NoContent();
//         }
//         catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
//         catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
//     }
//
//     /// <summary>Add a mentor to an event. Requires Admin role.</summary>
//     [HttpPost("{id:guid}/mentors")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     public async Task<IActionResult> AddMentor(Guid id, [FromBody] AddMentorRequest request, CancellationToken ct)
//     {
//         try
//         {
//             await eventsService.AddMentorAsync(id, request, ct);
//             return NoContent();
//         }
//         catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
//         catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
//     }
// }
