// Core type definitions matching the backend DTOs

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  name: string;
  role: "Admin" | "Judge" | "Mentor" | "Participant";
}

export interface EventResponse {
  id: string;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
  leaderboardVisible: boolean;
  createdAt: string;
}

export interface TeamResponse {
  id: string;
  eventId: string;
  name: string;
  repoUrl?: string;
  demoUrl?: string;
  description?: string;
  memberCount: number;
  createdAt: string;
}

export interface CriterionResponse {
  id: string;
  eventId: string;
  name: string;
  description?: string;
  weight: number;
  createdAt: string;
}

export interface ScoreResponse {
  id: string;
  judgeId: string;
  judgeName: string;
  teamId: string;
  teamName: string;
  criterionId: string;
  criterionName: string;
  value: number;
  comment?: string;
  createdAt: string;
}

export interface CriterionBreakdown {
  criterionId: string;
  criterionName: string;
  weight: number;
  averageRawScore: number;
  weightedScore: number;
}

export interface TeamResult {
  teamId: string;
  teamName: string;
  normalizedScore: number;
  rawScore: number;
  rank: number;
  breakdown: CriterionBreakdown[];
}

export interface EventResultsResponse {
  eventId: string;
  eventName: string;
  isVisible: boolean;
  rankings: TeamResult[];
}

export type MentorRequestStatus =
  | "Waiting"
  | "Assigned"
  | "InProgress"
  | "Done"
  | "Cancelled";

export interface MentorRequestResponse {
  id: string;
  eventId: string;
  teamId: string;
  teamName: string;
  assignedMentorId?: string;
  assignedMentorName?: string;
  topic: string;
  description?: string;
  status: MentorRequestStatus;
  priority: number;
  createdAt: string;
  assignedAt?: string;
  completedAt?: string;
}
