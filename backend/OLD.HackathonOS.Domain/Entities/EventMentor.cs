namespace HackathonOS.Domain.Entities;

public class EventMentor : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid MentorId { get; set; }
    public User Mentor { get; set; } = null!;

    public bool IsAvailable { get; set; } = true;
    public ICollection<string> Expertise { get; set; } = new List<string>();
}
