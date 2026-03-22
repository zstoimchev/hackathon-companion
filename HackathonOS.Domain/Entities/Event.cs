using System;
using System.Collections.Generic;

namespace HackathonOS.Domain.Entities;

public class Event : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool LeaderboardVisible { get; set; } = false;

    // Navigation properties
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
    public ICollection<EventJudge> EventJudges { get; set; } = new List<EventJudge>();
    public ICollection<EventMentor> EventMentors { get; set; } = new List<EventMentor>();
    public ICollection<MentorRequest> MentorRequests { get; set; } = new List<MentorRequest>();
}
