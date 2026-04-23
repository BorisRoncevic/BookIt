using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using VenueBooking.Service.Application.Interfaces;
using VenueBooking.Service.Models;
using VenueBooking.Service.Application.Services;

namespace VenueBooking.Service.Controller;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly VenueService _service;

    public VenuesController(VenueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Venue>>> GetAll()
    {
        var venues = await _service.GetAllAsync();
        return Ok(venues);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Venue>> GetById(Guid id)
    {
        var venue = await _service.GetByIdAsync(id);

        if (venue == null)
            return NotFound();

        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Venue venue)
    {
        await _service.CreateAsync(venue);

        return CreatedAtAction(
            nameof(GetById),
            new { id = venue.Id },
            venue
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] Venue venue)
    {
        if (id != venue.Id)
            return BadRequest();

        var success = await _service.UpdateAsync(id, venue);
        if (!success)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/venues/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
