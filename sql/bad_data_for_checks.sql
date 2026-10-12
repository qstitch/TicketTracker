-- Deliberately bad rows for testing the data-quality checks in reports.sql.
-- Run seed_data.sql first, then this file. Each row should be flagged by
-- exactly one check. Do NOT run this against real data.

INSERT INTO "Tickets" ("Title", "Description", "Severity", "Status", "ResolutionNotes", "CreatedAtUtc", "ResolvedAtUtc") VALUES
-- Check 4: reopened ticket that kept its resolved timestamp
('Reopened bug', '', 'High', 'InProgress', NULL, NOW() - INTERVAL '3 days', NOW() - INTERVAL '1 day'),
-- Check 5: closed ticket with blank resolution notes
('No notes', '', 'Low', 'Closed', '   ', NOW() - INTERVAL '3 days', NOW() - INTERVAL '2 days'),
-- Check 6: duplicate of seeded 'Login page returns 500' (different case and spacing)
('  login page returns 500 ', '', 'Critical', 'Open', NULL, NOW() - INTERVAL '1 day', NULL),
-- Check 7: resolved ticket with no resolved timestamp
('No timestamp', '', 'Low', 'Resolved', 'Done', NOW() - INTERVAL '3 days', NULL),
-- Check 8: resolved before it was created
('Time travel', '', 'Low', 'Resolved', 'Done', NOW() - INTERVAL '1 day', NOW() - INTERVAL '5 days');
