using Microsoft.AspNetCore.Mvc;
using Venue.Service.Application.Interfaces;
using VenueBooking.Service.Models;

namespace Venue.Service.Controller;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueRepository _repository;

    public VenuesController(IVenueRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Venue>>> GetAll()
    {
        var venues = await _repository.GetAllAsync();
        return Ok(venues);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Venue>> GetById(Guid id)
    {
        var venue = await _repository.GetByIdAsync(id);

        if (venue == null)
            return NotFound();

        return Ok(venue);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Venue venue)
    {
        await _repository.AddAsync(venue);

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

        var exists = await _repository.ExistsAsync(id);
        if (!exists)
            return NotFound();

        await _repository.UpdateAsync(venue);

        return NoContent();
    }

    // DELETE: api/venues/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
            return NotFound();

        await _repository.DeleteAsync(id);

        return NoContent();
    }
}