# 🏆 Hackathon Companion

> The operating system for running hackathons fairly and efficiently.

A production-ready platform for managing hackathon judging, scoring, and mentor coordination — built on **ASP.NET Core 10** and **Next.js 16**.

---

## ✨ Key Features

### ⚖️ Bias-Corrected Smart Judging
- Judges submit scores (1–10) per criterion per team
- Scores are **normalized per judge** using z-score normalization — harsh or lenient judges are automatically corrected
- Criterion weights are applied (e.g. Impact 50%, Implementation 30%)
- Judge weights can be configured (Sponsor judges can carry more weight)
- Final ranking is always fair

### 👨‍🏫 Mentor Queue System
- Teams submit help requests with topic and priority
- Mentors see a real-time queue, self-assign, and mark requests as done
- Like a support ticket system — "Uber for mentors"

### 🗂️ Project Submission Portal
- Teams register with name, repo URL, demo URL
- Admin manages everything through the panel

### 📊 Live Leaderboard
- Real-time rankings with per-criterion breakdown
- Leaderboard visibility toggled by admin (hidden during judging, revealed at finals)

---

## 🏗️ Architecture

```
hackathon-companion/
├── backend/
│   ├── HackathonOS.Domain/          # Entities, Enums (no dependencies)
│   ├── HackathonOS.Application/     # Services, DTOs, Interfaces, ScoringEngine
│   ├── HackathonOS.Infrastructure/  # EF Core, Repositories, JWT service
│   └── HackathonOS.Api/             # ASP.NET Core Web API, Controllers
│   └── HackathonOS.Tests/           # xUnit tests for scoring algorithm
└── frontend/                        # Next.js 16 + Tailwind CSS
```

### Tech Stack

| Layer | Technology                                 |
|-------|--------------------------------------------|
| Backend | ASP.NET Core 10, C# 12                     |
| ORM | Entity Framework Core 8 + Npgsql           |
| Database | PostgreSQL 16                              |
| Auth | JWT Bearer tokens, BCrypt password hashing |
| Frontend | Next.js 16, TypeScript, Tailwind CSS       |
| State | Zustand                                    |
| Containers | Docker + Docker Compose                    |
| CI | GitHub Actions                             |

---

## 🚀 Quick Start (Docker Compose)

```bash
# Clone and start everything
git clone https://github.com/gdgoncampus-up/hackathon-os.git
cd hackathon-os

docker compose up --build
```

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Frontend**: http://localhost:3000

---

## 🛠️ Local Development

### Prerequisites
- .NET 10 SDK
- Node.js 22+
- PostgreSQL 16 (or Docker)

### Backend

```bash
# Start PostgreSQL (via Docker)
docker compose up db -d

# Run API
cd src/HackathonOS.Api
dotnet run
# API available at http://localhost:5000
```

### Frontend

```bash
cd frontend
npm install
npm run dev
# Frontend at http://localhost:3000
```

### Run Tests

```bash
dotnet test HackathonOS.slnx
```

---

## 📐 Scoring Algorithm

The bias-corrected scoring works as follows:

1. **Group scores by judge**
2. **Compute per-judge mean and standard deviation**
3. **Normalize each score**: `z = (score - judge_mean) / judge_std`
    - If a judge gives all identical scores (std = 0), their contribution is 0
4. **Apply judge weight** (configurable per event)
5. **Apply criterion weight** (configurable per event)
6. **Sum contributions per team**
7. **Rank teams descending**

This ensures a harsh judge giving 3/10 is treated the same as a lenient judge giving 9/10, if they're both expressing the same relative preference.

---

## 🔐 Roles

| Role | Can Do |
|------|--------|
| **Admin** | Create events, teams, criteria; assign judges/mentors; toggle leaderboard |
| **Judge** | Submit scores for assigned events |
| **Mentor** | View queue, accept/complete mentor requests |
| **Participant** | View leaderboard (when visible), submit mentor requests |

---

## 📡 API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/auth/register` | Register user |
| POST | `/api/auth/login` | Login (returns JWT) |
| GET | `/api/events` | List events |
| POST | `/api/events` | Create event (Admin) |
| POST | `/api/events/{id}/judges` | Add judge (Admin) |
| GET | `/api/teams/by-event/{eventId}` | Get teams |
| POST | `/api/teams` | Register team (Admin) |
| GET | `/api/criteria/by-event/{eventId}` | Get criteria |
| POST | `/api/criteria` | Create criterion (Admin) |
| POST | `/api/scores` | Submit/update score (Judge) |
| GET | `/api/scores/my-scores/{eventId}` | Get my scores |
| GET | `/api/scores/results/{eventId}` | Get leaderboard |
| POST | `/api/mentorrequests` | Create help request |
| GET | `/api/mentorrequests/queue/{eventId}` | Get queue (Mentor) |
| PATCH | `/api/mentorrequests/{id}/assign` | Accept request (Mentor) |
| PATCH | `/api/mentorrequests/{id}/status` | Update status |

Full interactive docs at `/swagger` when running in Development mode.

---

## ⚙️ Configuration

Set these environment variables in production:

```env
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...
Jwt__Secret=<at-least-32-char-secret>
Jwt__Issuer=hackathon-os
Jwt__Audience=hackathon-os
AllowedOrigins__0=https://your-frontend.com
```

---

## 🧪 First Run Workflow

1. Register an Admin account: `POST /api/auth/register` with `role: "Admin"`
2. Create an event
3. Add teams + criteria (with weights)
4. Register judges + assign them to the event
5. Judges log in at `/judge` and submit scores
6. View bias-corrected results at `/leaderboard`
7. Toggle leaderboard visibility in Admin panel when ready to reveal
