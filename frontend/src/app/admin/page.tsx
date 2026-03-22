"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { api } from "@/lib/api";
import { EventResponse, TeamResponse, CriterionResponse } from "@/types";
import { hydrateAuth } from "@/store/auth";
import { useCallback } from "react";

export default function AdminPage() {
  const [events, setEvents] = useState<EventResponse[]>([]);
  const [selectedEvent, setSelectedEvent] = useState<string>("");
  const [teams, setTeams] = useState<TeamResponse[]>([]);
  const [criteria, setCriteria] = useState<CriterionResponse[]>([]);

  // New event form
  const [newEventName, setNewEventName] = useState("");
  const [newEventDesc, setNewEventDesc] = useState("");
  const [newEventStart, setNewEventStart] = useState("");
  const [newEventEnd, setNewEventEnd] = useState("");
  const [eventMsg, setEventMsg] = useState("");

  // New team form
  const [newTeamName, setNewTeamName] = useState("");
  const [newTeamRepo, setNewTeamRepo] = useState("");
  const [newTeamDemo, setNewTeamDemo] = useState("");
  const [teamMsg, setTeamMsg] = useState("");

  // New criterion form
  const [newCritName, setNewCritName] = useState("");
  const [newCritWeight, setNewCritWeight] = useState("");
  const [critMsg, setCritMsg] = useState("");


  const loadEvents = useCallback(() => {
    api.get<EventResponse[]>("/events").then((r) => setEvents(r.data));
  }, []);

  useEffect(() => {
    hydrateAuth();
    loadEvents();
  }, [loadEvents]);

  useEffect(() => {
    if (!selectedEvent) return;
    Promise.all([
      api.get<TeamResponse[]>(`/teams/by-event/${selectedEvent}`),
      api.get<CriterionResponse[]>(`/criteria/by-event/${selectedEvent}`),
    ]).then(([t, c]) => {
      setTeams(t.data);
      setCriteria(c.data);
    });
  }, [selectedEvent]);

  async function createEvent(e: React.FormEvent) {
    e.preventDefault();
    setEventMsg("");
    try {
      await api.post("/events", {
        name: newEventName,
        description: newEventDesc,
        startDate: newEventStart,
        endDate: newEventEnd,
      });
      setEventMsg("✅ Event created!");
      setNewEventName("");
      setNewEventDesc("");
      setNewEventStart("");
      setNewEventEnd("");
      loadEvents();
    } catch {
      setEventMsg("❌ Failed to create event.");
    }
  }

  async function createTeam(e: React.FormEvent) {
    e.preventDefault();
    setTeamMsg("");
    try {
      await api.post("/teams", {
        eventId: selectedEvent,
        name: newTeamName,
        repoUrl: newTeamRepo || null,
        demoUrl: newTeamDemo || null,
        memberCount: 0,
      });
      setTeamMsg("✅ Team created!");
      setNewTeamName("");
      setNewTeamRepo("");
      setNewTeamDemo("");
      const t = await api.get<TeamResponse[]>(`/teams/by-event/${selectedEvent}`);
      setTeams(t.data);
    } catch {
      setTeamMsg("❌ Failed to create team.");
    }
  }

  async function createCriterion(e: React.FormEvent) {
    e.preventDefault();
    setCritMsg("");
    try {
      await api.post("/criteria", {
        eventId: selectedEvent,
        name: newCritName,
        weight: parseFloat(newCritWeight),
      });
      setCritMsg("✅ Criterion created!");
      setNewCritName("");
      setNewCritWeight("");
      const c = await api.get<CriterionResponse[]>(`/criteria/by-event/${selectedEvent}`);
      setCriteria(c.data);
    } catch {
      setCritMsg("❌ Failed to create criterion.");
    }
  }

  async function toggleLeaderboard(evt: EventResponse) {
    await api.put(`/events/${evt.id}`, {
      ...evt,
      leaderboardVisible: !evt.leaderboardVisible,
    });
    loadEvents();
  }

  const totalWeight = criteria.reduce((s, c) => s + c.weight, 0);

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-indigo-700 mb-8">⚙️ Admin Panel</h1>

      {/* Create Event */}
      <section className="bg-white shadow rounded-xl p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">Create Event</h2>
        {eventMsg && <Msg text={eventMsg} />}
        <form onSubmit={createEvent} className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <FormInput label="Event Name" value={newEventName} onChange={setNewEventName} required />
          <FormInput label="Description" value={newEventDesc} onChange={setNewEventDesc} />
          <FormInput label="Start Date" type="datetime-local" value={newEventStart} onChange={setNewEventStart} required />
          <FormInput label="End Date" type="datetime-local" value={newEventEnd} onChange={setNewEventEnd} required />
          <div className="sm:col-span-2">
            <button className="bg-indigo-700 text-white px-6 py-2 rounded-lg hover:bg-indigo-800">
              Create Event
            </button>
          </div>
        </form>
      </section>

      {/* Manage Events */}
      <section className="bg-white shadow rounded-xl p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">Events</h2>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-gray-500 border-b">
                <th className="py-2">Name</th>
                <th>Start</th>
                <th>End</th>
                <th>Status</th>
                <th>Leaderboard</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {events.map((e) => (
                <tr key={e.id} className="border-b hover:bg-gray-50">
                  <td className="py-2 font-medium">{e.name}</td>
                  <td>{new Date(e.startDate).toLocaleDateString()}</td>
                  <td>{new Date(e.endDate).toLocaleDateString()}</td>
                  <td>
                    <span className={`px-2 py-0.5 rounded text-xs ${e.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-500"}`}>
                      {e.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td>
                    <button
                      onClick={() => toggleLeaderboard(e)}
                      className={`text-xs px-2 py-0.5 rounded ${e.leaderboardVisible ? "bg-blue-100 text-blue-700" : "bg-gray-100 text-gray-500"}`}
                    >
                      {e.leaderboardVisible ? "Visible" : "Hidden"}
                    </button>
                  </td>
                  <td>
                    <button
                      onClick={() => setSelectedEvent(e.id)}
                      className="text-indigo-600 hover:underline text-xs mr-2"
                    >
                      Manage
                    </button>
                    <Link
                      href={`/leaderboard?event=${e.id}`}
                      className="text-green-600 hover:underline text-xs"
                    >
                      Results
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      {/* Manage selected event */}
      {selectedEvent && (
        <>
          <div className="mb-4 p-3 bg-indigo-50 rounded-lg text-sm text-indigo-700">
            Managing: <strong>{events.find((e) => e.id === selectedEvent)?.name}</strong>
          </div>

          {/* Teams */}
          <section className="bg-white shadow rounded-xl p-6 mb-6">
            <h2 className="text-xl font-semibold mb-4">Teams</h2>
            {teamMsg && <Msg text={teamMsg} />}
            <form onSubmit={createTeam} className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4">
              <FormInput label="Team Name" value={newTeamName} onChange={setNewTeamName} required />
              <FormInput label="Repo URL" value={newTeamRepo} onChange={setNewTeamRepo} />
              <FormInput label="Demo URL" value={newTeamDemo} onChange={setNewTeamDemo} />
              <div className="sm:col-span-3">
                <button className="bg-indigo-700 text-white px-5 py-2 rounded-lg hover:bg-indigo-800 text-sm">
                  Add Team
                </button>
              </div>
            </form>
            <div className="space-y-2">
              {teams.map((t) => (
                <div key={t.id} className="flex items-center gap-3 p-2 bg-gray-50 rounded text-sm">
                  <span className="font-medium">{t.name}</span>
                  {t.repoUrl && <a href={t.repoUrl} target="_blank" className="text-indigo-600 text-xs hover:underline" rel="noreferrer">Repo</a>}
                  {t.demoUrl && <a href={t.demoUrl} target="_blank" className="text-green-600 text-xs hover:underline" rel="noreferrer">Demo</a>}
                </div>
              ))}
            </div>
          </section>

          {/* Criteria */}
          <section className="bg-white shadow rounded-xl p-6">
            <h2 className="text-xl font-semibold mb-1">Scoring Criteria</h2>
            <p className="text-sm text-gray-500 mb-3">
              Total weight: <strong className={totalWeight > 1.01 ? "text-red-600" : "text-green-600"}>{(totalWeight * 100).toFixed(0)}%</strong>
              {" "}{totalWeight > 1.01 && "(Exceeds 100%!)"}
            </p>
            {critMsg && <Msg text={critMsg} />}
            <form onSubmit={createCriterion} className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4">
              <FormInput label="Criterion Name" value={newCritName} onChange={setNewCritName} required />
              <FormInput label="Weight (0.0–1.0)" type="number" value={newCritWeight} onChange={setNewCritWeight} required />
              <div className="flex items-end">
                <button className="bg-indigo-700 text-white px-5 py-2 rounded-lg hover:bg-indigo-800 text-sm w-full">
                  Add Criterion
                </button>
              </div>
            </form>
            <div className="space-y-2">
              {criteria.map((c) => (
                <div key={c.id} className="flex items-center gap-3 p-2 bg-gray-50 rounded text-sm">
                  <span className="font-medium">{c.name}</span>
                  <span className="text-gray-500">{(c.weight * 100).toFixed(0)}%</span>
                </div>
              ))}
            </div>
          </section>
        </>
      )}
    </div>
  );
}

function FormInput({
  label,
  value,
  onChange,
  type = "text",
  required,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  type?: string;
  required?: boolean;
}) {
  return (
    <div>
      <label className="block text-sm font-medium mb-1">{label}</label>
      <input
        type={type}
        className="w-full border rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required={required}
      />
    </div>
  );
}

function Msg({ text }: { text: string }) {
  const isOk = text.startsWith("✅");
  return (
    <div
      className={`mb-3 p-2 rounded text-sm ${isOk ? "bg-green-50 text-green-700" : "bg-red-50 text-red-700"}`}
    >
      {text}
    </div>
  );
}
