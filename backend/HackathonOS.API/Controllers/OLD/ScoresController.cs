// using System.Security.Claims;
// using HackathonOS.Application.DTOs.Results;
// using HackathonOS.Application.DTOs.Scores;
// using HackathonOS.Application.Services;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace HackathonOS.Api.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
// [Authorize]
// public class ScoresController : ControllerBase
// {
//     private readonly ScoreService _scores;
//
//     public ScoresController(ScoreService scores) => _scores = scores;
//
//     /// <summary>Submit or update a score. Judge role required.</summary>
//     [HttpPost]
//     [Authorize(Roles = "Judge,Admin")]
//     [ProducesResponseType(typeof(ScoreResponse), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<IActionResult> Submit([FromBody] SubmitScoreRequest request, CancellationToken ct)
//     {
//         var judgeId = GetCurrentUserId();
//         try
//         {
//             var result = await _scores.SubmitAsync(judgeId, request, ct);
//             return Ok(result);
//         }
//         catch (ArgumentOutOfRangeException ex) { return BadRequest(new { error = ex.Message }); }
//         catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
//     }
//
//     /// <summary>Get scores submitted by the current judge for an event.</summary>
//     [HttpGet("my-scores/{eventId:guid}")]
//     [Authorize(Roles = "Judge,Admin")]
//     [ProducesResponseType(typeof(IEnumerable<ScoreResponse>), StatusCodes.Status200OK)]
//     public async Task<IActionResult> GetMyScores(Guid eventId, CancellationToken ct)
//     {
//         var judgeId = GetCurrentUserId();
//         var scores = await _scores.GetByJudgeAndEventAsync(judgeId, eventId, ct);
//         return Ok(scores);
//     }
//
//     /// <summary>Get bias-corrected results for an event. Admin always sees it; others only when leaderboard is visible.</summary>
//     [HttpGet("results/{eventId:guid}")]
//     [ProducesResponseType(typeof(EventResultsResponse), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status403Forbidden)]
//     public async Task<IActionResult> GetResults(Guid eventId, CancellationToken ct)
//     {
//         bool isAdmin = User.IsInRole("Admin");
//         try
//         {
//             var results = await _scores.GetResultsAsync(eventId, isAdmin, ct);
//             return Ok(results);
//         }
//         catch (UnauthorizedAccessException) { return Forbid(); }
//         catch (KeyNotFoundException) { return NotFound(); }
//     }
//
//     private Guid GetCurrentUserId()
//     {
//         var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                ?? User.FindFirst("sub")?.Value;
//         return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
//     }
// }
