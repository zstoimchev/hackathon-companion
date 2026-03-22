"use client";

import { useEffect, useState } from "react";
import { api } from "@/lib/api";
import {
  EventResponse,
  TeamResponse,
  CriterionResponse,
  ScoreResponse,
} from "@/types";
import { useAuthStore } from "@/store/auth";
import { hydrateAuth } from "@/store/auth";

export default function JudgePage() {
  const { user } = useAuthStore();
  const [events, setEvents] = useState<EventResponse[]>([]);
  const [selectedEvent, setSelectedEvent] = useState<string>("");
  const [teams, setTeams] = useState<TeamResponse[]>([]);
  const [criteria, setCriteria] = useState<CriterionResponse[]>([]);
  const [myScores, setMyScores] = useState<ScoreResponse[]>([]);
  const [scores, setScores] = useState<Record<string, Record<string, string>>>(
    {}
  );
  const [comments, setComments] = useState<Record<string, Record<string, string>>>({});
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");

  useEffect(() => {
    hydrateAuth();
  }, []);

  useEffect(() => {
    api.get<EventResponse[]>("/events").then((r) => setEvents(r.data));
  }, []);

  useEffect(() => {
    if (!selectedEvent) return;
    Promise.all([
      api.get<TeamResponse[]>(`/teams/by-event/${selectedEvent}`),
      api.get<CriterionResponse[]>(`/criteria/by-event/${selectedEvent}`),
      api.get<ScoreResponse[]>(`/scores/my-scores/${selectedEvent}`),
    ]).then(([t, c, s]) => {
      setTeams(t.data);
      setCriteria(c.data);
      setMyScores(s.data);
      // Pre-fill existing scores
      const scoreMap: Record<string, Record<string, string>> = {};
      const commentMap: Record<string, Record<string, string>> = {};
      s.data.forEach((score) => {
        if (!scoreMap[score.teamId]) scoreMap[score.teamId] = {};
        scoreMap[score.teamId][score.criterionId] = String(score.value);
        if (!commentMap[score.teamId]) commentMap[score.teamId] = {};
        commentMap[score.teamId][score.criterionId] = score.comment ?? "";
      });
      setScores(scoreMap);
      setComments(commentMap);
    });
  }, [selectedEvent]);

  function setScore(teamId: string, criterionId: string, value: string) {
    setScores((prev) => ({
      ...prev,
      [teamId]: { ...prev[teamId], [criterionId]: value },
    }));
  }

  function setComment(teamId: string, criterionId: string, value: string) {
    setComments((prev) => ({
      ...prev,
      [teamId]: { ...prev[teamId], [criterionId]: value },
    }));
  }

  async function submitAll() {
    setSaving(true);
    setMessage("");
    try {
      const requests = [];
      for (const team of teams) {
        for (const crit of criteria) {
          const val = scores[team.id]?.[crit.id];
          if (val !== undefined && val !== "") {
            requests.push(
              api.post("/scores", {
                teamId: team.id,
                criterionId: crit.id,
                value: parseFloat(val),
                comment: comments[team.id]?.[crit.id] ?? null,
              })
            );
          }
        }
      }
      await Promise.all(requests);
      setMessage("✅ Scores saved successfully!");
    } catch {
      setMessage("❌ Failed to save some scores.");
    } finally {
      setSaving(false);
    }
  }

  const scoreCount = myScores.length;
  const totalPossible = teams.length * criteria.length;

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-indigo-700 mb-6">
        Judge Dashboard
      </h1>
      {user && (
        <p className="text-gray-600 mb-4">
          Welcome, <strong>{user.name}</strong>
        </p>
      )}

      <div className="mb-6">
        <label className="block text-sm font-medium mb-1">Select Event</label>
        <select
          className="border rounded-lg px-3 py-2 w-full max-w-xs"
          value={selectedEvent}
          onChange={(e) => setSelectedEvent(e.target.value)}
        >
          <option value="">-- Choose an event --</option>
          {events.map((e) => (
            <option key={e.id} value={e.id}>
              {e.name}
            </option>
          ))}
        </select>
      </div>

      {selectedEvent && teams.length === 0 && (
        <p className="text-gray-500">No teams registered for this event yet.</p>
      )}

      {selectedEvent && teams.length > 0 && (
        <>
          <div className="mb-4 text-sm text-gray-500">
            Progress: {scoreCount} / {totalPossible} scores submitted
          </div>

          {message && (
            <div className="mb-4 p-3 rounded border text-sm bg-green-50 border-green-200 text-green-700">
              {message}
            </div>
          )}

          <div className="space-y-6">
            {teams.map((team) => (
              <div
                key={team.id}
                className="bg-white shadow rounded-xl p-6"
              >
                <div className="flex justify-between items-start mb-3">
                  <div>
                    <h2 className="text-lg font-bold">{team.name}</h2>
                    {team.description && (
                      <p className="text-gray-500 text-sm">{team.description}</p>
                    )}
                  </div>
                  <div className="text-right text-xs text-gray-400 space-y-1">
                    {team.repoUrl && (
                      <a
                        href={team.repoUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="block text-indigo-600 hover:underline"
                      >
                        📁 Repo
                      </a>
                    )}
                    {team.demoUrl && (
                      <a
                        href={team.demoUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="block text-indigo-600 hover:underline"
                      >
                        🚀 Demo
                      </a>
                    )}
                  </div>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  {criteria.map((crit) => (
                    <div key={crit.id} className="space-y-1">
                      <label className="text-sm font-medium">
                        {crit.name}{" "}
                        <span className="text-gray-400 font-normal text-xs">
                          (weight: {(crit.weight * 100).toFixed(0)}%)
                        </span>
                      </label>
                      <div className="flex gap-2 items-center">
                        <input
                          type="number"
                          min={1}
                          max={10}
                          step={0.5}
                          className="border rounded px-2 py-1 w-20 text-center focus:ring-2 focus:ring-indigo-500"
                          value={scores[team.id]?.[crit.id] ?? ""}
                          onChange={(e) =>
                            setScore(team.id, crit.id, e.target.value)
                          }
                          placeholder="1–10"
                        />
                        <input
                          type="text"
                          className="border rounded px-2 py-1 flex-1 text-sm focus:ring-2 focus:ring-indigo-500"
                          placeholder="Optional comment…"
                          value={comments[team.id]?.[crit.id] ?? ""}
                          onChange={(e) =>
                            setComment(team.id, crit.id, e.target.value)
                          }
                        />
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>

          <div className="mt-8 flex items-center gap-4">
            <button
              onClick={submitAll}
              disabled={saving}
              className="bg-indigo-700 text-white px-8 py-3 rounded-lg font-semibold hover:bg-indigo-800 disabled:opacity-50"
            >
              {saving ? "Saving…" : "Save All Scores"}
            </button>
            <span className="text-sm text-gray-500">
              Scores are automatically updated if you re-submit.
            </span>
          </div>
        </>
      )}
    </div>
  );
}
