using HackathonOS.Domain.Enums;

namespace HackathonOS.Domain.Entities;

public class MentorRequest : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid? AssignedMentorId { get; set; }
    public User? AssignedMentor { get; set; }

    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MentorRequestStatus Status { get; set; } = MentorRequestStatus.Waiting;
    public int Priority { get; set; } = 0;

    public DateTime? AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
