using HackathonOS.Application.DTOs.Events;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Services;

public class EventService
{
    private readonly IEventRepository _events;
    private readonly IUserRepository _users;

    public EventService(IEventRepository events, IUserRepository users)
    {
        _events = events;
        _users = users;
    }

    public async Task<IEnumerable<EventResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var events = await _events.GetAllAsync(ct);
        return events.Select(MapToResponse);
    }

    public async Task<EventResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Event {id} not found.");
        return MapToResponse(evt);
    }

    public async Task<EventResponse> CreateAsync(CreateEventRequest request, CancellationToken ct = default)
    {
        var evt = new Event
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        await _events.AddAsync(evt, ct);
        await _events.SaveChangesAsync(ct);
        return MapToResponse(evt);
    }

    public async Task<EventResponse> UpdateAsync(Guid id, UpdateEventRequest request, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Event {id} not found.");

        evt.Name = request.Name;
        evt.Description = request.Description;
        evt.StartDate = request.StartDate;
        evt.EndDate = request.EndDate;
        evt.IsActive = request.IsActive;
        evt.LeaderboardVisible = request.LeaderboardVisible;
        evt.UpdatedAt = DateTime.UtcNow;

        _events.Update(evt);
        await _events.SaveChangesAsync(ct);
        return MapToResponse(evt);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Event {id} not found.");
        _events.Remove(evt);
        await _events.SaveChangesAsync(ct);
    }

    public async Task AddJudgeAsync(Guid eventId, AddJudgeRequest request, CancellationToken ct = default)
    {
        var evt = await _events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");

        if (evt.EventJudges.Any(ej => ej.JudgeId == request.JudgeId))
            throw new InvalidOperationException("Judge already assigned to this event.");

        var judge = await _users.GetByIdAsync(request.JudgeId, ct)
            ?? throw new KeyNotFoundException($"User {request.JudgeId} not found.");

        evt.EventJudges.Add(new EventJudge
        {
            EventId = eventId,
            JudgeId = request.JudgeId,
            Weight = request.Weight
        });
        _events.Update(evt);
        await _events.SaveChangesAsync(ct);
    }

    public async Task AddMentorAsync(Guid eventId, AddMentorRequest request, CancellationToken ct = default)
    {
        var evt = await _events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");

        if (evt.EventMentors.Any(em => em.MentorId == request.MentorId))
            throw new InvalidOperationException("Mentor already assigned to this event.");

        var mentor = await _users.GetByIdAsync(request.MentorId, ct)
            ?? throw new KeyNotFoundException($"User {request.MentorId} not found.");

        evt.EventMentors.Add(new EventMentor
        {
            EventId = eventId,
            MentorId = request.MentorId
        });
        _events.Update(evt);
        await _events.SaveChangesAsync(ct);
    }

    private static EventResponse MapToResponse(Event e) => new(
        e.Id, e.Name, e.Description,
        e.StartDate, e.EndDate,
        e.IsActive, e.LeaderboardVisible,
        e.CreatedAt);
}
