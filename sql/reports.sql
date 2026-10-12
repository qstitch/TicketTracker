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

-- =====================================================================
-- Data-quality checks
-- Each check should return ZERO rows on healthy data.
-- Any rows returned are records to investigate.
-- =====================================================================

-- 4. Active tickets with a resolved timestamp
-- A ticket that is Open or InProgress should not have ResolvedAtUtc set.
-- Catches the reopen bug documented in docs/INCIDENT-001.md.
SELECT "Id", "Title", "Status", "ResolvedAtUtc"
FROM "Tickets"
WHERE "Status" IN ('Open', 'InProgress')
  AND "ResolvedAtUtc" IS NOT NULL
ORDER BY "Id";

-- 5. Resolved or closed tickets missing resolution notes
-- The API requires notes when resolving, so any rows here came from
-- another path (for example, a manual database edit or an import).
SELECT "Id", "Title", "Status", "ResolutionNotes"
FROM "Tickets"
WHERE "Status" IN ('Resolved', 'Closed')
  AND ("ResolutionNotes" IS NULL OR TRIM("ResolutionNotes") = '')
ORDER BY "Id";

-- 6. Duplicate ticket titles
-- Possible duplicate reports of the same issue. Matching ignores
-- capitalization and extra spaces at the start or end.
SELECT LOWER(TRIM("Title")) AS normalized_title,
       COUNT(*) AS ticket_count,
       STRING_AGG("Id"::text, ', ' ORDER BY "Id") AS ticket_ids
FROM "Tickets"
GROUP BY LOWER(TRIM("Title"))
HAVING COUNT(*) > 1
ORDER BY ticket_count DESC, normalized_title;

-- 7. Resolved or closed tickets missing a resolved timestamp
-- Every ticket that has been resolved should record when.
SELECT "Id", "Title", "Status", "ResolvedAtUtc"
FROM "Tickets"
WHERE "Status" IN ('Resolved', 'Closed')
  AND "ResolvedAtUtc" IS NULL
ORDER BY "Id";

-- 8. Timestamps that are impossible
-- Resolved before created, or created in the future.
SELECT "Id", "Title", "CreatedAtUtc", "ResolvedAtUtc"
FROM "Tickets"
WHERE "ResolvedAtUtc" < "CreatedAtUtc"
   OR "CreatedAtUtc" > NOW()
ORDER BY "Id";

-- Summary: one row per check with its count of problem records.
-- Run this first for a quick health overview, then run the
-- individual check above for any count that is not zero.
SELECT '4. Active tickets with resolved timestamp' AS check_name, COUNT(*) AS problem_rows
FROM "Tickets"
WHERE "Status" IN ('Open', 'InProgress') AND "ResolvedAtUtc" IS NOT NULL
UNION ALL
SELECT '5. Resolved/closed without resolution notes', COUNT(*)
FROM "Tickets"
WHERE "Status" IN ('Resolved', 'Closed')
  AND ("ResolutionNotes" IS NULL OR TRIM("ResolutionNotes") = '')
UNION ALL
SELECT '6. Titles with duplicates', COUNT(*)
FROM (
    SELECT 1 FROM "Tickets"
    GROUP BY LOWER(TRIM("Title"))
    HAVING COUNT(*) > 1
) AS dupes
UNION ALL
SELECT '7. Resolved/closed without resolved timestamp', COUNT(*)
FROM "Tickets"
WHERE "Status" IN ('Resolved', 'Closed') AND "ResolvedAtUtc" IS NULL
UNION ALL
SELECT '8. Impossible timestamps', COUNT(*)
FROM "Tickets"
WHERE "ResolvedAtUtc" < "CreatedAtUtc" OR "CreatedAtUtc" > NOW();