-- Run these in psql or any Postgres client against the "tickets" database.
-- Write them yourself first, then compare; this file is here so you can practice real SQL.

-- 1. Open work by severity
SELECT "Severity", COUNT(*) AS open_tickets
FROM "Tickets"
WHERE "Status" IN ('Open', 'InProgress')
GROUP BY "Severity"
ORDER BY open_tickets DESC;

-- 2. Average hours to resolve, by severity
SELECT "Severity",
       ROUND(AVG(EXTRACT(EPOCH FROM ("ResolvedAtUtc" - "CreatedAtUtc")) / 3600)::numeric, 2) AS avg_hours_to_resolve
FROM "Tickets"
WHERE "ResolvedAtUtc" IS NOT NULL
GROUP BY "Severity";

-- 3. Tickets still unresolved after 7 days
SELECT "Id", "Title", "Severity", "CreatedAtUtc"
FROM "Tickets"
WHERE "Status" IN ('Open', 'InProgress')
  AND "CreatedAtUtc" < NOW() - INTERVAL '7 days'
ORDER BY "CreatedAtUtc";
