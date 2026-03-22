namespace HackathonOS.Application.DTOs.Criteria;

public record CreateCriterionRequest(
    Guid EventId,
    string Name,
    string? Description,
    double Weight);

public record UpdateCriterionRequest(
    string Name,
    string? Description,
    double Weight);

public record CriterionResponse(
    Guid Id,
    Guid EventId,
    string Name,
    string? Description,
    double Weight,
    DateTime CreatedAt);
