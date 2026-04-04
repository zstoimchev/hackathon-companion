namespace HackathonOS.Application.DTOs.Teams;

public record CreateTeamRequest(
    Guid EventId,
    string Name,
    string? RepoUrl,
    string? DemoUrl,
    string? Description,
    int MemberCount);

public record UpdateTeamRequest(
    string Name,
    string? RepoUrl,
    string? DemoUrl,
    string? Description,
    int MemberCount);

public record TeamResponse(
    Guid Id,
    Guid EventId,
    string Name,
    string? RepoUrl,
    string? DemoUrl,
    string? Description,
    int MemberCount,
    DateTime CreatedAt);