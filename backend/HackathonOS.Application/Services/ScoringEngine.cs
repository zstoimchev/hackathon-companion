using HackathonOS.Application.DTOs.Results;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Services;

/// <summary>
/// Implements bias-corrected score aggregation.
///
/// Algorithm:
///   1. Group all raw scores by judge.
///   2. Compute per-judge mean and standard deviation.
///   3. Normalize each score: z = (score - judge_mean) / judge_std
///      (If a judge has zero std-dev, their contribution is 0 to avoid division issues.)
///   4. Apply judge weight (from EventJudge.Weight).
///   5. Apply criterion weight.
///   6. Sum weighted-normalized scores per team.
///   7. Rank teams descending.
/// </summary>
public static class ScoringEngine
{
    public static EventResultsResponse ComputeResults(
        Event evt,
        IReadOnlyList<Score> scores,
        IReadOnlyList<EventJudge> eventJudges)
    {
        if (scores.Count == 0)
        {
            return new EventResultsResponse(
                evt.Guid, evt.Name, evt.LeaderboardVisible,
                Array.Empty<TeamResult>());
        }

        // Build lookup maps
        var criteriaById = evt.Criteria.ToDictionary(c => c.Guid);
        var judgeWeights = eventJudges.ToDictionary(ej => ej.JudgeId, ej => ej.Weight);
        var teamsById = evt.Teams.ToDictionary(t => t.Guid);

        // Step 1: Compute per-judge statistics
        var judgeScoreGroups = scores
            .GroupBy(s => s.JudgeId)
            .ToDictionary(g => g.Key, g => g.Select(s => s.Value).ToList());

        var judgeStats = judgeScoreGroups.ToDictionary(
            kvp => kvp.Key,
            kvp =>
            {
                var values = kvp.Value;
                double mean = values.Average();
                double variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
                double std = Math.Sqrt(variance);
                return (mean, std);
            });

        // Step 2: Normalize scores and accumulate per-team
        // teamCriterionRaw[teamId][criterionId] = list of raw scores (for breakdown)
        var teamCriterionRaw = new Dictionary<Guid, Dictionary<Guid, List<double>>>();
        // teamWeightedSum[teamId] = sum of (normalizedScore * judgeWeight * criterionWeight)
        var teamWeightedSum = new Dictionary<Guid, double>();

        foreach (var score in scores)
        {
            if (!criteriaById.TryGetValue(score.CriterionId, out var criterion)) continue;
            if (!teamsById.ContainsKey(score.TeamId)) continue;

            var (mean, std) = judgeStats[score.JudgeId];
            double normalized = std == 0 ? 0 : (score.Value - mean) / std;

            double judgeWeight = judgeWeights.TryGetValue(score.JudgeId, out var jw) ? jw : 1.0;
            double criterionWeight = criterion.Weight;

            double contribution = normalized * judgeWeight * criterionWeight;

            if (!teamWeightedSum.ContainsKey(score.TeamId))
                teamWeightedSum[score.TeamId] = 0;
            teamWeightedSum[score.TeamId] += contribution;

            // Collect raw for breakdown
            if (!teamCriterionRaw.TryGetValue(score.TeamId, out var byCriterion))
            {
                byCriterion = new Dictionary<Guid, List<double>>();
                teamCriterionRaw[score.TeamId] = byCriterion;
            }
            if (!byCriterion.TryGetValue(score.CriterionId, out var rawList))
            {
                rawList = new List<double>();
                byCriterion[score.CriterionId] = rawList;
            }
            rawList.Add(score.Value);
        }

        // Step 3: Build per-team raw averages for display purposes
        var teamRawAvg = scores
            .GroupBy(s => s.TeamId)
            .ToDictionary(g => g.Key, g => g.Select(s => s.Value).Average());

        // Step 4: Rank teams
        var rankings = teamWeightedSum
            .OrderByDescending(kvp => kvp.Value)
            .Select((kvp, index) =>
            {
                var teamId = kvp.Key;
                var team = teamsById[teamId];

                var breakdown = criteriaById.Values
                    .Select(c =>
                    {
                        double avgRaw = 0;
                        if (teamCriterionRaw.TryGetValue(teamId, out var byCrit) &&
                            byCrit.TryGetValue(c.Guid, out var rawList) &&
                            rawList.Count > 0)
                        {
                            avgRaw = rawList.Average();
                        }
                        return new CriterionBreakdown(
                            c.Guid, c.Name, c.Weight, avgRaw, avgRaw * c.Weight);
                    })
                    .OrderByDescending(b => b.Weight)
                    .ToList();

                return new TeamResult(
                    teamId,
                    team.Name,
                    Math.Round(kvp.Value, 4),
                    Math.Round(teamRawAvg.TryGetValue(teamId, out var raw) ? raw : 0, 2),
                    index + 1,
                    breakdown);
            })
            .ToList();

        return new EventResultsResponse(evt.Guid, evt.Name, evt.LeaderboardVisible, rankings);
    }
}
