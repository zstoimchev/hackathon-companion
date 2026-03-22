using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HackathonOS.Application.DTOs.MentorRequests;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Domain.Enums;

namespace HackathonOS.Application.Services;

public class MentorRequestService
{
    private readonly IMentorRequestRepository _requests;
    private readonly IEventRepository _events;
    private readonly IUserRepository _users;

    public MentorRequestService(
        IMentorRequestRepository requests,
        IEventRepository events,
        IUserRepository users)
    {
        _requests = requests;
        _events = events;
        _users = users;
    }

    public async Task<MentorRequestResponse> CreateAsync(CreateMentorRequestDto request, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException($"Event {request.EventId} not found.");
        _ = evt;

        var mentorReq = new MentorRequest
        {
            EventId = request.EventId,
            TeamId = request.TeamId,
            Topic = request.Topic,
            Description = request.Description,
            Priority = request.Priority,
            Status = MentorRequestStatus.Waiting
        };
        await _requests.AddAsync(mentorReq, ct);
        await _requests.SaveChangesAsync(ct);
        return await BuildResponseAsync(mentorReq, ct);
    }

    public async Task<IEnumerable<MentorRequestResponse>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var requests = await _requests.GetByEventAsync(eventId, ct);
        var result = new List<MentorRequestResponse>();
        foreach (var r in requests)
            result.Add(await BuildResponseAsync(r, ct));
        return result;
    }

    public async Task<IEnumerable<MentorRequestResponse>> GetWaitingAsync(Guid eventId, CancellationToken ct = default)
    {
        var requests = await _requests.GetWaitingAsync(eventId, ct);
        var result = new List<MentorRequestResponse>();
        foreach (var r in requests)
            result.Add(await BuildResponseAsync(r, ct));
        return result;
    }

    public async Task<MentorRequestResponse> AssignMentorAsync(Guid requestId, Guid mentorId, CancellationToken ct = default)
    {
        var req = await _requests.GetByIdAsync(requestId, ct)
            ?? throw new KeyNotFoundException($"MentorRequest {requestId} not found.");

        if (req.Status != MentorRequestStatus.Waiting)
            throw new InvalidOperationException("Only waiting requests can be assigned.");

        req.AssignedMentorId = mentorId;
        req.Status = MentorRequestStatus.Assigned;
        req.AssignedAt = DateTime.UtcNow;
        req.UpdatedAt = DateTime.UtcNow;

        _requests.Update(req);
        await _requests.SaveChangesAsync(ct);
        return await BuildResponseAsync(req, ct);
    }

    public async Task<MentorRequestResponse> UpdateStatusAsync(Guid requestId, MentorRequestStatus status, CancellationToken ct = default)
    {
        var req = await _requests.GetByIdAsync(requestId, ct)
            ?? throw new KeyNotFoundException($"MentorRequest {requestId} not found.");

        req.Status = status;
        if (status == MentorRequestStatus.Done)
            req.CompletedAt = DateTime.UtcNow;
        req.UpdatedAt = DateTime.UtcNow;

        _requests.Update(req);
        await _requests.SaveChangesAsync(ct);
        return await BuildResponseAsync(req, ct);
    }

    private async Task<MentorRequestResponse> BuildResponseAsync(MentorRequest r, CancellationToken ct)
    {
        string? mentorName = null;
        if (r.AssignedMentorId.HasValue)
        {
            var mentor = await _users.GetByIdAsync(r.AssignedMentorId.Value, ct);
            mentorName = mentor?.Name;
        }

        // Load team name if not navigation-loaded
        string teamName = r.Team?.Name ?? string.Empty;

        return new MentorRequestResponse(
            r.Id, r.EventId, r.TeamId, teamName,
            r.AssignedMentorId, mentorName,
            r.Topic, r.Description, r.Status, r.Priority,
            r.CreatedAt, r.AssignedAt, r.CompletedAt);
    }
}
