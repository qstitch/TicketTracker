namespace TicketTracker.Api.Models;

public enum TicketStatus { Open, InProgress, Resolved, Closed }

public enum Severity { Low, Medium, High, Critical }

public class Ticket
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public Severity Severity { get; set; } = Severity.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public string? ResolutionNotes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}
