namespace HackathonOS.Application.DTOs.Results;

public record TeamResult(
    Guid TeamId,
    string TeamName,
    double NormalizedScore,
    double RawScore,
    int Rank,
    IReadOnlyList<CriterionBreakdown> Breakdown);

public record CriterionBreakdown(
    Guid CriterionId,
    string CriterionName,
    double Weight,
    double AverageRawScore,
    double WeightedScore);

public record EventResultsResponse(
    Guid EventId,
    string EventName,
    bool IsVisible,
    IReadOnlyList<TeamResult> Rankings);
