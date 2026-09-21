-- Ticket Tracker: SQL practice queries (PostgreSQL)
-- Written while learning SQL against the Tickets table.
-- Load sql/seed_data.sql first so these queries return sample rows.

-- 1. Critical or High severity tickets (filtering with OR)
SELECT *
FROM "Tickets"
WHERE "Severity" = 'High' OR "Severity" = 'Critical';

-- 2a. Active work: Open or InProgress tickets, newest first (filtering with IN, sorting)
SELECT "Id", "Title", "Severity"
FROM "Tickets"
WHERE "Status" IN ('Open', 'InProgress')
ORDER BY "CreatedAtUtc" DESC;

-- 2b. Every ticket that is not Closed, newest first (<> also includes Resolved tickets)
SELECT "Id", "Title", "Status"
FROM "Tickets"
WHERE "Status" <> 'Closed'
ORDER BY "CreatedAtUtc" DESC;

-- 3. Ticket count per severity (GROUP BY with COUNT)
SELECT "Severity", COUNT(*) AS severity_count
FROM "Tickets"
GROUP BY "Severity"
ORDER BY severity_count DESC;

-- 4. Tickets not resolved yet, two ways
-- 4a. Using the timestamp
SELECT "Title"
FROM "Tickets"
WHERE "ResolvedAtUtc" IS NULL;

-- 4b. Using the status
SELECT "Title"
FROM "Tickets"
WHERE "Status" <> 'Resolved';

-- Finding: 4a and 4b return different results. 4b also returns Closed tickets,
-- because a ticket must pass through Resolved before it is Closed.
-- Comparing the two queries also exposed a bug: reopened tickets kept their
-- ResolvedAtUtc value. Fixed in TicketService, with a regression test and
-- data-quality check 4 in sql/reports.sql.
