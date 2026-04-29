using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenueBooking.Service.Application.Interfaces;
using VenueBooking.Service.Models;

namespace VenueBooking.Service.Controller;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _service;

    public VenuesController(IVenueService service)
    {
        _service = service;
    }

    [HttpGet("amenities")]
    public async Task<ActionResult<List<AmenityDto>>> GetAmenities()
    {
        var amenities = await _service.GetAmenitiesAsync();
        return Ok(amenities.Select(ToAmenityDto));
    }

    [HttpGet]
    public async Task<ActionResult<List<VenueResponse>>> GetAll()
    {
        var venues = await _service.GetAllAsync();
        return Ok(venues.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VenueResponse>> GetById(Guid id)
    {
        var venue = await _service.GetByIdAsync(id);

        if (venue == null)
            return NotFound();

        return Ok(ToResponse(venue));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<VenueResponse>> Create([FromBody] CreateVenueRequest request)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId == null)
            return Unauthorized();

        var venue = ToVenue(request);
        venue.OwnerId = ownerId.Value;
        await _service.CreateAsync(venue);
        var createdVenue = await _service.GetByIdAsync(venue.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = venue.Id },
            ToResponse(createdVenue ?? venue)
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateVenueRequest request)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId == null)
            return Unauthorized();

        var existingVenue = await _service.GetByIdAsync(id);
        if (existingVenue == null)
            return NotFound();

        if (existingVenue.OwnerId != ownerId.Value)
            return Forbid();

        var venue = ToVenue(request);

        var success = await _service.UpdateAsync(id, venue);
        if (!success)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/venues/{id}
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId == null)
            return Unauthorized();

        var existingVenue = await _service.GetByIdAsync(id);
        if (existingVenue == null)
            return NotFound();

        if (existingVenue.OwnerId != ownerId.Value)
            return Forbid();

        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    private static Venue ToVenue(VenueRequest request)
    {
        return new Venue
        {
            Title = request.Title,
            Description = request.Description,
            City = request.City,
            Country = request.Country,
            Address = request.Address,
            PricePerNight = request.PricePerNight,
            MaxGuests = request.MaxGuests,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Amenities = request.AmenityIds
                .Distinct()
                .Select(amenityId => new VenueAmenity { AmenityId = amenityId })
                .ToList()
        };
    }

    private static VenueResponse ToResponse(Venue venue)
    {
        return new VenueResponse(
            venue.Id,
            venue.OwnerId,
            venue.Title,
            venue.Description,
            venue.City,
            venue.Country,
            venue.Address,
            venue.PricePerNight,
            venue.MaxGuests,
            venue.Bedrooms,
            venue.Bathrooms,
            venue.IsActive,
            venue.CreatedAt,
            venue.Images.Select(image => new VenueImageDto(image.Id, image.Url)).ToList(),
            venue.Amenities
                .Where(venueAmenity => venueAmenity.Amenity != null)
                .Select(venueAmenity => ToAmenityDto(venueAmenity.Amenity!))
                .ToList()
        );
    }

    private static AmenityDto ToAmenityDto(Amenity amenity)
    {
        return new AmenityDto(amenity.Id, amenity.Name);
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}

public record AmenityDto(int Id, string Name);

public record VenueImageDto(Guid Id, string Url);

public record VenueResponse(
    Guid Id,
    Guid OwnerId,
    string Title,
    string Description,
    string City,
    string Country,
    string Address,
    decimal PricePerNight,
    int MaxGuests,
    int Bedrooms,
    int Bathrooms,
    bool IsActive,
    DateTime CreatedAt,
    List<VenueImageDto> Images,
    List<AmenityDto> Amenities
);

public record VenueRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal PricePerNight { get; init; }
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public List<int> AmenityIds { get; init; } = new();
}

public record CreateVenueRequest : VenueRequest;

public record UpdateVenueRequest : VenueRequest;
