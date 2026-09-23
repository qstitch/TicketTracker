using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Models;

namespace TicketTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var ticket = modelBuilder.Entity<Ticket>();
        ticket.Property(t => t.Title).HasMaxLength(120);
        // Store enums as readable text in the database instead of integers.
        ticket.Property(t => t.Status).HasConversion<string>();
        ticket.Property(t => t.Severity).HasConversion<string>();

        modelBuilder.Entity<Ticket>().HasIndex(t => t.Status);
    }

}
