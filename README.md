# Accounting app demo

A full-stack personal accounting app demo. Record, edit, and delete expense/income entries through a Unity UI, backed by a RESTful .NET API and MySQL.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Unity 6 · UI Toolkit · VContainer · UniTask · R3 · Newtonsoft.Json |
| Backend | .NET 9 · ASP.NET Core · MediatR (CQRS) · FluentValidation |
| Database | MySQL 8 (Docker) · EF Core 9 · Pomelo provider |

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Unity 6](https://unity.com/releases/editor/whats-new/6000.0.0) (open `frontend/unity/` in Unity Hub)

> .NET 9 SDK is only needed if you want to run the API outside of Docker.

## Getting Started

**1. Start the API and database**
```bash
docker compose up -d --build
# MySQL starts → health check passes → API starts → migrations apply automatically
# → http://localhost:5265
```

**2. Open the Unity project**

Open `frontend/unity/` in Unity Hub, load `Assets/Scenes/SampleScene.unity`, and press Play.

### Running the API without Docker (optional)

Requires [.NET 9 SDK](https://dotnet.microsoft.com/download). Run from `backend/`:

```bash
docker compose up -d          # MySQL only
dotnet run --project src/Api  # → http://localhost:5265 (migrations apply on startup)
```

API docs (Development only): `http://localhost:5265/scalar/v1`

## Architecture

### Backend — Clean Architecture

```
Domain ← Application ← Infrastructure
              ↑
             Api
```

Each CRUD operation lives in its own self-contained folder under `Application/AccountEntries/` with a command/query record, a MediatR handler, and an optional FluentValidation validator. Controllers only call `_mediator.Send(...)`. EF Core migrations are applied automatically on startup.

### Frontend — Unidirectional data flow

```
AccountingApiService  →  AccountingStore  →  EntryListScreen
  (HTTP / UniTask)       (R3 Reactive)       EntryFormScreen
```

VContainer wires everything together via `Main : LifetimeScope`. Both UI panels share a single `UIDocument`; visibility is driven by `AccountingStore.IsFormVisible`.

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/accountentries` | List all entries (ordered by date desc) |
| `GET` | `/api/accountentries/{id}` | Get single entry |
| `POST` | `/api/accountentries` | Create entry → `201 Created` |
| `PUT` | `/api/accountentries/{id}` | Update entry → `204 No Content` |
| `DELETE` | `/api/accountentries/{id}` | Delete entry → `204 No Content` |

**Entry fields:** `amount` (decimal), `category` (string, max 100), `note` (string, max 500), `date` (ISO 8601)

## Project Structure

```
curd-demo/
├── docker-compose.yml        # MySQL + API (one-command startup)
├── backend/
│   ├── Dockerfile
│   └── src/
│       ├── Domain/           # AccountEntry entity
│       ├── Application/      # CQRS handlers + validators
│       ├── Infrastructure/   # EF Core + MySQL
│       └── Api/              # Controllers + Program.cs
└── frontend/
    └── unity/
        └── Assets/
            ├── Scripts/Runtime/
            │   ├── Api/      # HTTP service + DTOs
            │   ├── Store/    # Reactive state
            │   └── UI/       # Screen presenters
            └── UI/           # UXML + USS
```
