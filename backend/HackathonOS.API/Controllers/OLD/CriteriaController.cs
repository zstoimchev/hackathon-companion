// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace HackathonOS.Api.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// [Authorize]
// public class CriteriaController : ControllerBase
// {
//     private readonly CriterionService _criteria;
//
//     public CriteriaController(CriterionService criteria) => _criteria = criteria;
//
//     /// <summary>Get all criteria for an event.</summary>
//     [HttpGet("by-event/{eventId:guid}")]
//     [ProducesResponseType(typeof(IEnumerable<CriterionResponse>), StatusCodes.Status200OK)]
//     public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken ct)
//     {
//         try { return Ok(await _criteria.GetByEventAsync(eventId, ct)); }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     /// <summary>Create a new criterion. Requires Admin role.</summary>
//     [HttpPost]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(typeof(CriterionResponse), StatusCodes.Status201Created)]
//     public async Task<IActionResult> Create([FromBody] CreateCriterionRequest request, CancellationToken ct)
//     {
//         try
//         {
//             var result = await _criteria.CreateAsync(request, ct);
//             return CreatedAtAction(nameof(GetByEvent), new { eventId = result.EventId }, result);
//         }
//         catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
//     }
//
//     /// <summary>Update a criterion. Requires Admin role.</summary>
//     [HttpPut("{id:guid}")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(typeof(CriterionResponse), StatusCodes.Status200OK)]
//     public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCriterionRequest request, CancellationToken ct)
//     {
//         try { return Ok(await _criteria.UpdateAsync(id, request, ct)); }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     /// <summary>Delete a criterion. Requires Admin role.</summary>
//     [HttpDelete("{id:guid}")]
//     [Authorize(Roles = "Admin")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//     {
//         try
//         {
//             await _criteria.DeleteAsync(id, ct);
//             return NoContent();
//         }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
// }
