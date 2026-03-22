using System.Collections.Generic;
using HackathonOS.Domain.Enums;

namespace HackathonOS.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Score> Scores { get; set; } = new List<Score>();
    public ICollection<MentorRequest> AssignedRequests { get; set; } = new List<MentorRequest>();
    public ICollection<EventJudge> EventJudges { get; set; } = new List<EventJudge>();
    public ICollection<EventMentor> EventMentors { get; set; } = new List<EventMentor>();
}
