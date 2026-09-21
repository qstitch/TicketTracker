-- Sample data for practicing the queries in practice.sql and reports.sql.
-- Run this ONCE against the "tickets" database. Running it again adds duplicate rows,
-- which the duplicate-title check in reports.sql would then flag.

INSERT INTO "Tickets" ("Title", "Description", "Severity", "Status", "ResolutionNotes", "CreatedAtUtc", "ResolvedAtUtc") VALUES
('Login page returns 500', 'Users cannot sign in', 'Critical', 'Resolved', 'Fixed null reference in auth handler', NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days 20 hours'),
('Report export times out', 'CSV export hangs on large files', 'High', 'InProgress', NULL, NOW() - INTERVAL '8 days', NULL),
('Typo on billing page', 'Misspelled heading', 'Low', 'Open', NULL, NOW() - INTERVAL '2 days', NULL),
('Slow dashboard load', 'Takes 12 seconds to load', 'Medium', 'Open', NULL, NOW() - INTERVAL '9 days', NULL),
('Password reset email not sent', 'No email received', 'High', 'Resolved', 'SMTP credentials rotated', NOW() - INTERVAL '6 days', NOW() - INTERVAL '5 days'),
('Search returns duplicates', 'Same result appears twice', 'Medium', 'Closed', 'Added DISTINCT to query', NOW() - INTERVAL '14 days', NOW() - INTERVAL '12 days'),
('Mobile menu overlaps', 'Menu covers content on small screens', 'Low', 'InProgress', NULL, NOW() - INTERVAL '1 day', NULL);
