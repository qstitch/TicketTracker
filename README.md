# Ticket Tracker API

A RESTful web service for tracking software defects and support tickets, built with C# and ASP.NET Core (.NET 10). Tickets follow an enforced lifecycle (Open → In Progress → Resolved → Closed) with validation and business rules covered by automated tests.

## Tech stack
C# · ASP.NET Core · Entity Framework Core · PostgreSQL · xUnit · GitHub Actions · Docker (local database)

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
1. Start Postgres: `docker run --name tickets-db -e POSTGRES_PASSWORD=devpass -e POSTGRES_DB=tickets -p 5432:5432 -d postgres:17`
2. Run the API: `dotnet run --project src/TicketTracker.Api`
3. Run tests: `dotnet test`

## Reporting queries
`sql/reports.sql` contains example SQL queries against the tickets table.
`sql/reports.sql` has reports and data-quality checks. 
`sql/practice.sql` has basic SQL practice queries, and `sql/seed_data.sql` has sample data.

## Deployment
_TODO: add the live URL and deployment notes here once the AWS deployment is complete._
