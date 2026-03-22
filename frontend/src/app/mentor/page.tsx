"use client";

import { useEffect, useState, useCallback } from "react";
import { api } from "@/lib/api";
import { EventResponse, MentorRequestResponse } from "@/types";
import { hydrateAuth } from "@/store/auth";

const STATUS_COLORS: Record<string, string> = {
  Waiting: "bg-yellow-100 text-yellow-800",
  Assigned: "bg-blue-100 text-blue-800",
  InProgress: "bg-purple-100 text-purple-800",
  Done: "bg-green-100 text-green-800",
  Cancelled: "bg-gray-100 text-gray-500",
};

export default function MentorPage() {
  const [events, setEvents] = useState<EventResponse[]>([]);
  const [selectedEvent, setSelectedEvent] = useState("");
  const [requests, setRequests] = useState<MentorRequestResponse[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    hydrateAuth();
    api.get<EventResponse[]>("/events").then((r) => setEvents(r.data));
  }, []);

  const loadRequests = useCallback(async () => {
    if (!selectedEvent) return;

    setLoading(true);
    try {
      const r = await api.get<MentorRequestResponse[]>(
          `/mentorrequests/by-event/${selectedEvent}`
      );
      setRequests(r.data);
    } finally {
      setLoading(false);
    }
  }, [selectedEvent]);

  useEffect(() => {
    if (!selectedEvent) {
      setRequests([]);
      return;
    }

    const fetchRequests = async () => {
      setLoading(true);
      try {
        const r = await api.get<MentorRequestResponse[]>(
            `/mentorrequests/by-event/${selectedEvent}`
        );
        setRequests(r.data);
      } finally {
        setLoading(false);
      }
    };

    fetchRequests();
  }, [selectedEvent]);

  async function accept(id: string) {
    await api.patch(`/mentorrequests/${id}/assign`);
    loadRequests();
  }

  async function updateStatus(id: string, status: string) {
    await api.patch(`/mentorrequests/${id}/status`, { status });
    loadRequests();
  }

  const waiting = requests.filter((r) => r.status === "Waiting");
  const active = requests.filter((r) =>
    ["Assigned", "InProgress"].includes(r.status)
  );
  const done = requests.filter((r) =>
    ["Done", "Cancelled"].includes(r.status)
  );

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-indigo-700 mb-6">
        👨‍🏫 Mentor Queue
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

      {loading && <p className="text-gray-500">Loading queue…</p>}

      {selectedEvent && !loading && (
        <>
          <Section title={`⏳ Waiting (${waiting.length})`}>
            {waiting.length === 0 && (
              <p className="text-gray-400 text-sm">Queue is empty 🎉</p>
            )}
            {waiting.map((r) => (
              <RequestCard
                key={r.id}
                request={r}
                actions={
                  <button
                    onClick={() => accept(r.id)}
                    className="bg-blue-600 text-white px-4 py-1 rounded text-sm hover:bg-blue-700"
                  >
                    Accept
                  </button>
                }
              />
            ))}
          </Section>

          <Section title={`🔧 Active (${active.length})`}>
            {active.map((r) => (
              <RequestCard
                key={r.id}
                request={r}
                actions={
                  <div className="flex gap-2">
                    {r.status === "Assigned" && (
                      <button
                        onClick={() => updateStatus(r.id, "InProgress")}
                        className="bg-purple-600 text-white px-3 py-1 rounded text-sm hover:bg-purple-700"
                      >
                        Start
                      </button>
                    )}
                    <button
                      onClick={() => updateStatus(r.id, "Done")}
                      className="bg-green-600 text-white px-3 py-1 rounded text-sm hover:bg-green-700"
                    >
                      Done
                    </button>
                    <button
                      onClick={() => updateStatus(r.id, "Cancelled")}
                      className="bg-gray-400 text-white px-3 py-1 rounded text-sm hover:bg-gray-500"
                    >
                      Cancel
                    </button>
                  </div>
                }
              />
            ))}
          </Section>

          <Section title={`✅ Completed (${done.length})`}>
            {done.map((r) => (
              <RequestCard key={r.id} request={r} />
            ))}
          </Section>
        </>
      )}
    </div>
  );
}

function Section({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <div className="mb-8">
      <h2 className="text-lg font-semibold mb-3">{title}</h2>
      <div className="space-y-3">{children}</div>
    </div>
  );
}

function RequestCard({
  request,
  actions,
}: {
  request: MentorRequestResponse;
  actions?: React.ReactNode;
}) {
  return (
    <div className="bg-white shadow rounded-lg p-4 flex items-start justify-between gap-4">
      <div className="flex-1">
        <div className="flex items-center gap-2 mb-1">
          <span
            className={`text-xs font-semibold px-2 py-0.5 rounded-full ${STATUS_COLORS[request.status]}`}
          >
            {request.status}
          </span>
          {request.priority > 0 && (
            <span className="text-xs text-orange-600 font-semibold">
              🔥 P{request.priority}
            </span>
          )}
          <span className="text-xs text-gray-400">
            {new Date(request.createdAt).toLocaleTimeString()}
          </span>
        </div>
        <div className="font-medium">{request.topic}</div>
        <div className="text-sm text-gray-500">Team: {request.teamName}</div>
        {request.description && (
          <div className="text-sm text-gray-500 mt-1">{request.description}</div>
        )}
        {request.assignedMentorName && (
          <div className="text-xs text-indigo-600 mt-1">
            👤 {request.assignedMentorName}
          </div>
        )}
      </div>
      {actions && <div className="shrink-0">{actions}</div>}
    </div>
  );
}
