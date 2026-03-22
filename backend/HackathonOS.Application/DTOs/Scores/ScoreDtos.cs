namespace HackathonOS.Application.DTOs.Scores;

public record SubmitScoreRequest(
    Guid TeamId,
    Guid CriterionId,
    double Value,
    string? Comment);

public record ScoreResponse(
    Guid Id,
    Guid JudgeId,
    string JudgeName,
    Guid TeamId,
    string TeamName,
    Guid CriterionId,
    string CriterionName,
    double Value,
    string? Comment,
    DateTime CreatedAt);
