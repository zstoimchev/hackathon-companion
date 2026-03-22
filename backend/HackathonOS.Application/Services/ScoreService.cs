using HackathonOS.Application.DTOs.Scores;
using HackathonOS.Application.DTOs.Results;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Services;

public class ScoreService
{
    private readonly IScoreRepository _scores;
    private readonly IEventRepository _events;
    private readonly IRepository<Team> _teams;
    private readonly IRepository<Criterion> _criteria;

    public ScoreService(
        IScoreRepository scores,
        IEventRepository events,
        IRepository<Team> teams,
        IRepository<Criterion> criteria)
    {
        _scores = scores;
        _events = events;
        _teams = teams;
        _criteria = criteria;
    }

    public async Task<ScoreResponse> SubmitAsync(Guid judgeId, SubmitScoreRequest request, CancellationToken ct = default)
    {
        if (request.Value is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(request.Value), "Score must be between 1 and 10.");

        var team = await _teams.GetByIdAsync(request.TeamId, ct)
            ?? throw new KeyNotFoundException($"Team {request.TeamId} not found.");

        var criterion = await _criteria.GetByIdAsync(request.CriterionId, ct)
            ?? throw new KeyNotFoundException($"Criterion {request.CriterionId} not found.");

        // Upsert: if judge already scored this team/criterion, update
        var existing = await _scores.GetExistingAsync(judgeId, request.TeamId, request.CriterionId, ct);
        if (existing is not null)
        {
            existing.Value = request.Value;
            existing.Comment = request.Comment;
            existing.UpdatedAt = DateTime.UtcNow;
            _scores.Update(existing);
            await _scores.SaveChangesAsync(ct);
            return MapToResponse(existing, "Updated", team.Name, criterion.Name);
        }

        var score = new Score
        {
            JudgeId = judgeId,
            TeamId = request.TeamId,
            CriterionId = request.CriterionId,
            Value = request.Value,
            Comment = request.Comment
        };
        await _scores.AddAsync(score, ct);
        await _scores.SaveChangesAsync(ct);

        return MapToResponse(score, "Created", team.Name, criterion.Name);
    }

    public async Task<IEnumerable<ScoreResponse>> GetByJudgeAndEventAsync(Guid judgeId, Guid eventId, CancellationToken ct = default)
    {
        var scores = await _scores.GetByJudgeAndEventAsync(judgeId, eventId, ct);
        return scores.Select(s => MapToResponse(s,
            s.Judge?.Name ?? string.Empty,
            s.Team?.Name ?? string.Empty,
            s.Criterion?.Name ?? string.Empty));
    }

    public async Task<EventResultsResponse> GetResultsAsync(Guid eventId, bool isAdmin, CancellationToken ct = default)
    {
        var evt = await _events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");

        if (!isAdmin && !evt.LeaderboardVisible)
            throw new UnauthorizedAccessException("Leaderboard is not yet visible.");

        var scores = (await _scores.GetByEventAsync(eventId, ct)).ToList();
        var eventJudges = (await _events.GetEventJudgesAsync(eventId, ct)).ToList();

        return ScoringEngine.ComputeResults(evt, scores, eventJudges);
    }

    private static ScoreResponse MapToResponse(Score s, string judgeName, string teamName, string criterionName) => new(
        s.Id, s.JudgeId, judgeName, s.TeamId, teamName, s.CriterionId, criterionName,
        s.Value, s.Comment, s.CreatedAt);
}
