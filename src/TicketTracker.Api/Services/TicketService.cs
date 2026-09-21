using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models;

namespace TicketTracker.Api.Services;

public class TicketService(AppDbContext db)
{
    public async Task<Ticket> CreateAsync(CreateTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required.");
        if (request.Title.Trim().Length > 120)
            throw new ArgumentException("Title must be 120 characters or fewer.");

        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? "",
            Severity = request.Severity
        };

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();
        return ticket;
    }

    public Task<List<Ticket>> ListAsync(TicketStatus? status = null) =>
        db.Tickets
            .Where(t => status == null || t.Status == status)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync();

    public async Task<Ticket?> GetAsync(int id) => await db.Tickets.FindAsync(id);

    public async Task<Ticket?> UpdateStatusAsync(int id, UpdateStatusRequest request)
    {
        var ticket = await db.Tickets.FindAsync(id);
        if (ticket is null) return null;

        if (!IsValidTransition(ticket.Status, request.Status))
            throw new InvalidOperationException(
                $"Cannot move ticket from {ticket.Status} to {request.Status}.");

        if (request.Status == TicketStatus.Resolved)
        {
            if (string.IsNullOrWhiteSpace(request.ResolutionNotes))
                throw new ArgumentException("Resolution notes are required when resolving a ticket.");

            ticket.ResolutionNotes = request.ResolutionNotes.Trim();
            ticket.ResolvedAtUtc = DateTime.UtcNow;
        }

        // Reopening a resolved ticket clears the resolved timestamp.
        if (ticket.Status == TicketStatus.Resolved && request.Status == TicketStatus.InProgress)
        {
            ticket.ResolvedAtUtc = null;
        }

        ticket.Status = request.Status;
        await db.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ticket = await db.Tickets.FindAsync(id);
        if (ticket is null) return false;

        db.Tickets.Remove(ticket);
        await db.SaveChangesAsync();
        return true;
    }

    // Allowed lifecycle: Open -> InProgress -> Resolved -> Closed (with reopen paths).
    public static bool IsValidTransition(TicketStatus from, TicketStatus to) => (from, to) switch
    {
        (TicketStatus.Open, TicketStatus.InProgress) => true,
        (TicketStatus.InProgress, TicketStatus.Open) => true,
        (TicketStatus.InProgress, TicketStatus.Resolved) => true,
        (TicketStatus.Resolved, TicketStatus.InProgress) => true,
        (TicketStatus.Resolved, TicketStatus.Closed) => true,
        _ => false
    };
}
