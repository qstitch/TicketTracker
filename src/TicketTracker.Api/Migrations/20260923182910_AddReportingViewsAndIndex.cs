using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReportingViewsAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW open_tickets_by_severity AS
                SELECT ""Severity"", COUNT(*) AS open_count
                FROM ""Tickets""
                WHERE ""Status"" NOT IN ('Resolved', 'Closed')
                GROUP BY ""Severity"";");

            migrationBuilder.Sql(@"
                CREATE VIEW stale_open_tickets AS
                SELECT ""Id"", ""Title"", ""Severity"", ""CreatedAtUtc""
                FROM ""Tickets""
                WHERE ""Status"" NOT IN ('Resolved', 'Closed')
                AND ""CreatedAtUtc"" < NOW() - INTERVAL '7 days';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS stale_open_tickets;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS open_tickets_by_severity;");
        }
    }
}
