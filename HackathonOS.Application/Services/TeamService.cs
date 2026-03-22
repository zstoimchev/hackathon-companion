using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HackathonOS.Application.DTOs.Teams;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Services;

public class TeamService
{
    private readonly IRepository<Team> _teams;
    private readonly IEventRepository _events;

    public TeamService(IRepository<Team> teams, IEventRepository events)
    {
        _teams = teams;
        _events = events;
    }

    public async Task<IEnumerable<TeamResponse>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var evt = await _events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");
        return evt.Teams.Select(MapToResponse);
    }

    public async Task<TeamResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var team = await _teams.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        return MapToResponse(team);
    }

    public async Task<TeamResponse> CreateAsync(CreateTeamRequest request, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException($"Event {request.EventId} not found.");

        var team = new Team
        {
            EventId = request.EventId,
            Name = request.Name,
            RepoUrl = request.RepoUrl,
            DemoUrl = request.DemoUrl,
            Description = request.Description,
            MemberCount = request.MemberCount
        };
        await _teams.AddAsync(team, ct);
        await _teams.SaveChangesAsync(ct);
        return MapToResponse(team);
    }

    public async Task<TeamResponse> UpdateAsync(Guid id, UpdateTeamRequest request, CancellationToken ct = default)
    {
        var team = await _teams.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Team {id} not found.");

        team.Name = request.Name;
        team.RepoUrl = request.RepoUrl;
        team.DemoUrl = request.DemoUrl;
        team.Description = request.Description;
        team.MemberCount = request.MemberCount;
        team.UpdatedAt = DateTime.UtcNow;

        _teams.Update(team);
        await _teams.SaveChangesAsync(ct);
        return MapToResponse(team);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var team = await _teams.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Team {id} not found.");
        _teams.Remove(team);
        await _teams.SaveChangesAsync(ct);
    }

    private static TeamResponse MapToResponse(Team t) => new(
        t.Id, t.EventId, t.Name,
        t.RepoUrl, t.DemoUrl,
        t.Description, t.MemberCount,
        t.CreatedAt);
}
