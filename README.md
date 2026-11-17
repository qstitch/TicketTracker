# Ticket Tracker API

[![CI](https://github.com/qstitch/TicketTracker/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/qstitch/TicketTracker/actions/workflows/ci.yml)

A RESTful web service for tracking software defects and support tickets, built with C# and ASP.NET Core (.NET 10). Tickets follow an enforced lifecycle (Open → In Progress → Resolved → Closed) with validation and business rules covered by automated tests.

## Tech stack
C# · ASP.NET Core · Entity Framework Core · PostgreSQL · xUnit · GitHub Actions · Docker · Docker Compose

## Endpoints
| Method | Route | Description |
|---|---|---|
| GET | `/api/tickets?status=Open` | List tickets, optionally filtered by status |
| GET | `/api/tickets/{id}` | Get one ticket |
| POST | `/api/tickets` | Create a ticket |
| PATCH | `/api/tickets/{id}/status` | Move a ticket through its lifecycle |
| DELETE | `/api/tickets/{id}` | Delete a ticket |
| GET | `/health` | Health check |

## Business rules
- Titles are required and limited to 120 characters.
- Status can only change along valid transitions (for example, Open cannot jump straight to Resolved).
- Resolving a ticket requires resolution notes and records a resolved timestamp.

## Run locally
1. Start Postgres: `docker run --name tickets-db -e POSTGRES_PASSWORD=devpass -e POSTGRES_DB=tickets -p 5432:5432 -d postgres:16`
2. Run the API: `dotnet run --project src/TicketTracker.Api`
3. Run tests: `dotnet test`

## Run with Docker
Runs the API and PostgreSQL together with Docker Compose. You only need [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

```bash
docker compose up --build
```

This builds the API image, starts PostgreSQL, waits for its health check to pass, and applies all EF Core migrations automatically (including the reporting views and the `Status` index). When the log shows `Now listening on: http://[::]:8080`, the API is available at `http://localhost:8080` (health check at `/health`, OpenAPI spec at `/openapi/v1.json`).

PostgreSQL is exposed on host port `5433` (user `postgres`, database `tickets`), so it doesn't clash with the container from "Run locally."

```bash
docker compose down        # stop and remove containers, keep the data
docker compose down -v     # also delete the database volume
```

Notes:
- The image uses a multi-stage build: the .NET SDK compiles the app, and only the output is copied into the smaller ASP.NET runtime image.
- The database password in `docker-compose.yml` is for local development only.
- A `libgssapi_krb5.so.2` warning in the API logs is harmless. It's the Npgsql driver looking for an optional Kerberos library.

## Reporting queries
`sql/reports.sql` has example reports and data-quality checks.
`sql/practice.sql` has basic SQL practice queries, and `sql/seed_data.sql` has sample data.
Run `sql/bad_data_for_checks.sql` after seeding to see each data-quality check catch a problem.

## Database Views
- `open_tickets_by_severity`: count of open tickets grouped by severity
- `stale_open_tickets`: open tickets created more than 7 days ago

Both views and an index on `Status` are created by EF Core migrations,
so `dotnet ef database update` builds the full schema from code.

## Deployment
_TODO: add the live URL and deployment notes here once the AWS deployment is complete._
