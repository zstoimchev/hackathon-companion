using HackathonOS.Domain.Enums;

namespace HackathonOS.Application.DTOs.MentorRequests;

public record CreateMentorRequestDto(
    Guid EventId,
    Guid TeamId,
    string Topic,
    string? Description,
    int Priority = 0);

public record MentorRequestResponse(
    Guid Id,
    Guid EventId,
    Guid TeamId,
    string TeamName,
    Guid? AssignedMentorId,
    string? AssignedMentorName,
    string Topic,
    string? Description,
    MentorRequestStatus Status,
    int Priority,
    DateTime CreatedAt,
    DateTime? AssignedAt,
    DateTime? CompletedAt);
