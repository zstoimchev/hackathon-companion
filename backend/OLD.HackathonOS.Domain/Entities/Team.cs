namespace HackathonOS.Domain.Entities;

public class Team : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? RepoUrl { get; set; }
    public string? DemoUrl { get; set; }
    public string? Description { get; set; }
    public int MemberCount { get; set; }

    // Navigation properties
    public ICollection<Score> Scores { get; set; } = new List<Score>();
    public ICollection<MentorRequest> MentorRequests { get; set; } = new List<MentorRequest>();
}
