using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookingS.Service.Application.Services;
using BookingS.Service.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingS.Service.Controller;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }


    [HttpPost]
    [Authorize]

    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var userEmail = GetUserEmail();
        if (string.IsNullOrWhiteSpace(userEmail))
            return BadRequest("User email is missing from token.");

        try
        {
            var booking = await _service.CreateBookingAsync(request, userId, userEmail);
            return Ok(booking);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("venue/{venueId}/booked-ranges")]
public async Task<IActionResult> GetBookedRanges(Guid venueId)
{
    var ranges = await _service.GetBookedRangesAsync(venueId);
    return Ok(ranges);
}

    [HttpGet("venue/{venueId}")]
    [Authorize]
    public async Task<IActionResult> GetVenueBookings(Guid venueId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var bookings = await _service.GetVenueBookingsForOwnerAsync(venueId, userId);

        if (bookings == null)
            return Forbid();

        return Ok(bookings);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyBookings()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var bookings = await _service.GetMyBookingsAsync(userId);

        return Ok(bookings);
    }

    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await _service.CancelAsync(id, userId, GetUserEmail());

        return result switch
        {
            CancelBookingResult.Cancelled => NoContent(),
            CancelBookingResult.NotFound => NotFound(),
            CancelBookingResult.Forbidden => Forbid(),
            CancelBookingResult.AlreadyCancelled => NoContent(),
            CancelBookingResult.PastBooking => BadRequest("Past bookings cannot be cancelled."),
            _ => BadRequest()
        };
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub");

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out userId))
            return true;

        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(email.ToLowerInvariant()));
            userId = new Guid(hash);
            return true;
        }

        userId = Guid.Empty;
        return false;
    }

    private string? GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value
               ?? User.FindFirst("email")?.Value;
    }
}
