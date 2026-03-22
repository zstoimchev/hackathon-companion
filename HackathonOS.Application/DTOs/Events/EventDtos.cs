using System;

namespace HackathonOS.Application.DTOs.Events;

public record CreateEventRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate);

public record UpdateEventRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool LeaderboardVisible);

public record EventResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool LeaderboardVisible,
    DateTime CreatedAt);

public record AddJudgeRequest(Guid JudgeId, double Weight = 1.0);
public record AddMentorRequest(Guid MentorId);
