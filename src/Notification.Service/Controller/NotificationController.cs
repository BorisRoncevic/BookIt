using Microsoft.AspNetCore.Mvc;

namespace Notification.Service.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [HttpPost("booking-created")]
    public async Task<IActionResult> BookingCreated([FromBody] BookingCreatedEvent e)
    {
        try
        {
            await _service.SendBookingCreatedAsync(e);
            return Ok(new { message = "Email sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Failed to send email",
                error = ex.Message
            });
        }
    }
}
