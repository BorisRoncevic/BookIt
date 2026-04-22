[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var booking = await _service.CreateBookingAsync(request);
        return Ok(booking);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _service.CancelAsync(id);
        return NoContent();
    }
}