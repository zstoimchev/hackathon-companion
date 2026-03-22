"use client";

import { useEffect, useState } from "react";
import { api } from "@/lib/api";
import { EventResponse, EventResultsResponse, TeamResult } from "@/types";
import { hydrateAuth } from "@/store/auth";
import axios from "axios";

export default function LeaderboardPage() {
  const [events, setEvents] = useState<EventResponse[]>([]);
  const [selectedEvent, setSelectedEvent] = useState("");
  const [results, setResults] = useState<EventResultsResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    hydrateAuth();
    api.get<EventResponse[]>("/events").then((r) => setEvents(r.data));
  }, []);

    useEffect(() => {
        if (!selectedEvent) return;

        const fetchResults = async () => {
            setLoading(true);
            setError("");

            try {
                const r = await api.get<EventResultsResponse>(
                    `/scores/results/${selectedEvent}`
                );
                setResults(r.data);
            } catch (e: unknown) {
                if (axios.isAxiosError(e) && e.response?.status === 403) {
                    setError("Leaderboard is not yet visible for this event.");
                } else {
                    setError("Failed to load results.");
                }
                setResults(null);
            } finally {
                setLoading(false);
            }
        };

        fetchResults();
    }, [selectedEvent]);
  const medals = ["🥇", "🥈", "🥉"];

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-indigo-700 mb-6">
        📊 Live Leaderboard
      </h1>

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

      {loading && <p className="text-gray-500">Loading results…</p>}

      {error && (
        <div className="bg-yellow-50 border border-yellow-200 text-yellow-800 rounded p-4">
          {error}
        </div>
      )}

      {results && results.rankings.length === 0 && (
        <p className="text-gray-500">No scores submitted yet.</p>
      )}

      {results && results.rankings.length > 0 && (
        <div className="space-y-4">
          {results.rankings.map((team: TeamResult) => (
            <div
              key={team.teamId}
              className={`bg-white shadow rounded-xl p-5 border-l-4 ${
                team.rank === 1
                  ? "border-yellow-400"
                  : team.rank === 2
                  ? "border-gray-400"
                  : team.rank === 3
                  ? "border-amber-600"
                  : "border-indigo-200"
              }`}
            >
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center gap-3">
                  <span className="text-2xl">
                    {medals[team.rank - 1] ?? `#${team.rank}`}
                  </span>
                  <h2 className="text-lg font-bold">{team.teamName}</h2>
                </div>
                <div className="text-right">
                  <div className="text-sm text-gray-400">
                    Normalized Score
                  </div>
                  <div className="text-2xl font-mono font-bold text-indigo-700">
                    {team.normalizedScore.toFixed(3)}
                  </div>
                  <div className="text-xs text-gray-400">
                    Raw avg: {team.rawScore.toFixed(1)}
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-2 sm:grid-cols-3 gap-2 mt-3">
                {team.breakdown.map((b) => (
                  <div
                    key={b.criterionId}
                    className="bg-gray-50 rounded px-3 py-2 text-sm"
                  >
                    <div className="font-medium">{b.criterionName}</div>
                    <div className="text-gray-500">
                      Avg: {b.averageRawScore.toFixed(1)} ×{" "}
                      {(b.weight * 100).toFixed(0)}%
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
