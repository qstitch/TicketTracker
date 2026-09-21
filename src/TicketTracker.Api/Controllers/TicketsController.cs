using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Models;
using TicketTracker.Api.Services;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(TicketService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Ticket>>> List([FromQuery] TicketStatus? status) =>
        Ok(await service.ListAsync(status));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ticket>> Get(int id)
    {
        var ticket = await service.GetAsync(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> Create(CreateTicketRequest request)
    {
        try
        {
            var ticket = await service.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<Ticket>> UpdateStatus(int id, UpdateStatusRequest request)
    {
        try
        {
            var ticket = await service.UpdateStatusAsync(id, request);
            return ticket is null ? NotFound() : Ok(ticket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await service.DeleteAsync(id) ? NoContent() : NotFound();
}
