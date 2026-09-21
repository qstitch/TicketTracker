namespace TicketTracker.Api.Models;

public record CreateTicketRequest(string Title, string? Description, Severity Severity);

public record UpdateStatusRequest(TicketStatus Status, string? ResolutionNotes);
