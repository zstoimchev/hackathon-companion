using FluentAssertions;
using HackathonOS.Application.Services;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Tests;

public class ScoringEngineTests
{
    private static Event BuildEvent(params (Guid id, string name, double weight)[] criteria)
    {
        var evt = new Event
        {
            Guid = Guid.NewGuid(),
            Name = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        foreach (var (id, name, weight) in criteria)
        {
            evt.Criteria.Add(new Criterion { Guid = id, EventId = evt.Guid, Name = name, Weight = weight });
        }

        return evt;
    }

    private static Team AddTeam(Event evt, string name)
    {
        var team = new Team { Guid = Guid.NewGuid(), EventId = evt.Guid, Name = name };
        evt.Teams.Add(team);
        return team;
    }

    private static Score MakeScore(Guid judgeId, Team team, Criterion criterion, double value)
        => new() { JudgeId = judgeId, TeamId = team.Guid, CriterionId = criterion.Guid, Value = value };

    [Fact]
    public void EmptyScores_ReturnsEmptyRankings()
    {
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var result = ScoringEngine.ComputeResults(evt, Array.Empty<Score>(), Array.Empty<EventJudge>());

        result.Rankings.Should().BeEmpty();
    }

    [Fact]
    public void SingleJudge_SingleCriterion_RanksCorrectly()
    {
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var crit = evt.Criteria.First();

        var teamA = AddTeam(evt, "Alpha");
        var teamB = AddTeam(evt, "Beta");

        var judgeId = Guid.NewGuid();
        // Judge gives Alpha 8, Beta 6
        var scores = new List<Score>
        {
            MakeScore(judgeId, teamA, crit, 8),
            MakeScore(judgeId, teamB, crit, 6)
        };

        var result = ScoringEngine.ComputeResults(evt, scores, Array.Empty<EventJudge>());

        result.Rankings.Should().HaveCount(2);
        result.Rankings[0].TeamName.Should().Be("Alpha");
        result.Rankings[1].TeamName.Should().Be("Beta");
        result.Rankings[0].Rank.Should().Be(1);
        result.Rankings[1].Rank.Should().Be(2);
    }

    [Fact]
    public void BiasNormalization_HarshJudgeDoesNotDominate()
    {
        // Two judges: one is lenient (gives 9–10), one is harsh (gives 1–3).
        // Without normalization the harsh judge would heavily disadvantage a team.
        // With normalization both judge's scores should contribute fairly.
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var crit = evt.Criteria.First();

        var teamA = AddTeam(evt, "Alpha");
        var teamB = AddTeam(evt, "Beta");
        var teamC = AddTeam(evt, "Gamma");

        var lenientJudge = Guid.NewGuid();
        var harshJudge = Guid.NewGuid();

        // Lenient: A=10, B=9, C=8 (A is best)
        // Harsh:   A=3,  B=2,  C=1 (A is still best)
        var scores = new List<Score>
        {
            MakeScore(lenientJudge, teamA, crit, 10),
            MakeScore(lenientJudge, teamB, crit, 9),
            MakeScore(lenientJudge, teamC, crit, 8),
            MakeScore(harshJudge, teamA, crit, 3),
            MakeScore(harshJudge, teamB, crit, 2),
            MakeScore(harshJudge, teamC, crit, 1),
        };

        var result = ScoringEngine.ComputeResults(evt, scores, Array.Empty<EventJudge>());

        result.Rankings[0].TeamName.Should().Be("Alpha");
        result.Rankings[1].TeamName.Should().Be("Beta");
        result.Rankings[2].TeamName.Should().Be("Gamma");
    }

    [Fact]
    public void JudgeWithZeroStdDev_ContributesZero()
    {
        // A judge who gives all identical scores contributes 0 (all normalized to 0)
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var crit = evt.Criteria.First();

        var teamA = AddTeam(evt, "Alpha");
        var teamB = AddTeam(evt, "Beta");

        var flatJudge = Guid.NewGuid();
        var scores = new List<Score>
        {
            MakeScore(flatJudge, teamA, crit, 7),
            MakeScore(flatJudge, teamB, crit, 7),
        };

        var result = ScoringEngine.ComputeResults(evt, scores, Array.Empty<EventJudge>());
        // Both should have normalized score 0, so tie (order may vary)
        result.Rankings.Should().HaveCount(2);
        result.Rankings.All(r => r.NormalizedScore == 0).Should().BeTrue();
    }

    [Fact]
    public void CriterionWeights_AreApplied()
    {
        var impactId = Guid.NewGuid();
        var implId = Guid.NewGuid();
        // Impact 70%, Implementation 30%
        var evt = BuildEvent((impactId, "Impact", 0.7), (implId, "Implementation", 0.3));
        var impact = evt.Criteria.First(c => c.Guid == impactId);
        var impl = evt.Criteria.First(c => c.Guid == implId);

        var teamA = AddTeam(evt, "Alpha");
        var teamB = AddTeam(evt, "Beta");
        var judgeId = Guid.NewGuid();

        // Alpha: high Impact (10), low Implementation (2)
        // Beta:  low Impact (2), high Implementation (10)
        // Since Impact has 70% weight, Alpha should rank higher
        var scores = new List<Score>
        {
            MakeScore(judgeId, teamA, impact, 10),
            MakeScore(judgeId, teamA, impl, 2),
            MakeScore(judgeId, teamB, impact, 2),
            MakeScore(judgeId, teamB, impl, 10),
        };

        var result = ScoringEngine.ComputeResults(evt, scores, Array.Empty<EventJudge>());
        result.Rankings[0].TeamName.Should().Be("Alpha");
    }

    [Fact]
    public void JudgeWeights_AreRespected()
    {
        // Two judges: one with weight 2.0, one with weight 0.5
        // The heavy-weight judge's opinion should dominate
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var crit = evt.Criteria.First();

        var teamA = AddTeam(evt, "Alpha");
        var teamB = AddTeam(evt, "Beta");

        var heavyJudge = Guid.NewGuid();
        var lightJudge = Guid.NewGuid();

        // Heavy judge: prefers B (B=10, A=2)
        // Light judge: prefers A (A=10, B=2)
        var scores = new List<Score>
        {
            MakeScore(heavyJudge, teamA, crit, 2),
            MakeScore(heavyJudge, teamB, crit, 10),
            MakeScore(lightJudge, teamA, crit, 10),
            MakeScore(lightJudge, teamB, crit, 2),
        };

        var eventJudges = new List<EventJudge>
        {
            new() { JudgeId = heavyJudge, EventId = evt.Guid, Weight = 2.0 },
            new() { JudgeId = lightJudge, EventId = evt.Guid, Weight = 0.5 }
        };

        var result = ScoringEngine.ComputeResults(evt, scores, eventJudges);
        // Heavy judge prefers B, so B should rank first
        result.Rankings[0].TeamName.Should().Be("Beta");
    }

    [Fact]
    public void Results_IncludesCriterionBreakdown()
    {
        var critId = Guid.NewGuid();
        var evt = BuildEvent((critId, "Impact", 1.0));
        var crit = evt.Criteria.First();

        var teamA = AddTeam(evt, "Alpha");
        var judgeId = Guid.NewGuid();

        var scores = new List<Score> { MakeScore(judgeId, teamA, crit, 8) };
        var result = ScoringEngine.ComputeResults(evt, scores, Array.Empty<EventJudge>());

        result.Rankings[0].Breakdown.Should().HaveCount(1);
        result.Rankings[0].Breakdown[0].CriterionName.Should().Be("Impact");
        result.Rankings[0].Breakdown[0].AverageRawScore.Should().Be(8);
    }
}
