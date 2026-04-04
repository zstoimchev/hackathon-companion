using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<Event?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken ct = default);
    Task<IEnumerable<EventJudge>> GetEventJudgesAsync(Guid eventId, CancellationToken ct = default);
    Task<IEnumerable<EventMentor>> GetEventMentorsAsync(Guid eventId, CancellationToken ct = default);
}
