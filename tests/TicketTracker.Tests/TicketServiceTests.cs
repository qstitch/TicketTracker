using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Data;
using TicketTracker.Api.Models;
using TicketTracker.Api.Services;
using Xunit;

namespace TicketTracker.Tests;

public class TicketServiceTests
{
    // Each test gets its own isolated in-memory database.
    private static TicketService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TicketService(new AppDbContext(options));
    }

    [Fact]
    public async Task Reopen_ClearsResolvedTimestamp()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(new CreateTicketRequest("Bug", null, Severity.Medium));
        await service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.InProgress, null));
        await service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.Resolved, "Fixed"));

        var reopened = await service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.InProgress, null));

        Assert.Null(reopened!.ResolvedAtUtc);
    }

    [Fact]
    public async Task Create_ValidRequest_DefaultsToOpenStatus()
    {
        var service = CreateService();

        var ticket = await service.CreateAsync(new CreateTicketRequest("Login fails", "500 error", Severity.High));

        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.True(ticket.Id > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_BlankTitle_ThrowsArgumentException(string title)
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateTicketRequest(title, null, Severity.Low)));
    }

    [Fact]
    public async Task Create_TitleTooLong_ThrowsArgumentException()
    {
        var service = CreateService();
        var longTitle = new string('x', 121);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateTicketRequest(longTitle, null, Severity.Low)));
    }

    [Fact]
    public async Task UpdateStatus_SkippingInProgress_ThrowsInvalidOperation()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(new CreateTicketRequest("Bug", null, Severity.Medium));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.Resolved, "Fixed")));
    }

    [Fact]
    public async Task Resolve_WithoutNotes_ThrowsArgumentException()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(new CreateTicketRequest("Bug", null, Severity.Medium));
        await service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.InProgress, null));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.Resolved, " ")));
    }

    [Fact]
    public async Task Resolve_WithNotes_SetsResolvedTimestampAndNotes()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(new CreateTicketRequest("Bug", null, Severity.Medium));
        await service.UpdateStatusAsync(ticket.Id, new UpdateStatusRequest(TicketStatus.InProgress, null));

        var resolved = await service.UpdateStatusAsync(ticket.Id,
            new UpdateStatusRequest(TicketStatus.Resolved, "Patched null check"));

        Assert.NotNull(resolved);
        Assert.Equal(TicketStatus.Resolved, resolved!.Status);
        Assert.Equal("Patched null check", resolved.ResolutionNotes);
        Assert.NotNull(resolved.ResolvedAtUtc);
    }

    [Fact]
    public async Task UpdateStatus_UnknownId_ReturnsNull()
    {
        var service = CreateService();

        var result = await service.UpdateStatusAsync(999, new UpdateStatusRequest(TicketStatus.InProgress, null));

        Assert.Null(result);
    }

    [Fact]
    public async Task List_FiltersByStatus()
    {
        var service = CreateService();
        var first = await service.CreateAsync(new CreateTicketRequest("A", null, Severity.Low));
        await service.CreateAsync(new CreateTicketRequest("B", null, Severity.Low));
        await service.UpdateStatusAsync(first.Id, new UpdateStatusRequest(TicketStatus.InProgress, null));

        var inProgress = await service.ListAsync(TicketStatus.InProgress);
        var open = await service.ListAsync(TicketStatus.Open);

        Assert.Single(inProgress);
        Assert.Single(open);
    }

    [Fact]
    public async Task Delete_ExistingTicket_RemovesIt()
    {
        var service = CreateService();
        var ticket = await service.CreateAsync(new CreateTicketRequest("Temp", null, Severity.Low));

        var deleted = await service.DeleteAsync(ticket.Id);

        Assert.True(deleted);
        Assert.Null(await service.GetAsync(ticket.Id));
    }

    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.InProgress, true)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved, true)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed, true)]
    [InlineData(TicketStatus.Resolved, TicketStatus.InProgress, true)]
    [InlineData(TicketStatus.Open, TicketStatus.Closed, false)]
    [InlineData(TicketStatus.Closed, TicketStatus.Open, false)]
    [InlineData(TicketStatus.Open, TicketStatus.Resolved, false)]
    public void IsValidTransition_ReturnsExpectedResult(TicketStatus from, TicketStatus to, bool expected)
    {
        Assert.Equal(expected, TicketService.IsValidTransition(from, to));
    }
}
