namespace HackathonOS.Domain.Entities;

public class EventJudge : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid JudgeId { get; set; }
    public User Judge { get; set; } = null!;

    /// <summary>
    /// Optional weight multiplier for this judge's scores within the event.
    /// 1.0 = normal weight, 0.5 = half weight, etc.
    /// </summary>
    public double Weight { get; set; } = 1.0;
}
