using System.Net.Http.Json;
using BookingS.Service.Model;

namespace BookingS.Service.Application.Services;

public class NotificationClient : INotificationClient
{
    private readonly HttpClient _httpClient;

    public NotificationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendBookingCreatedAsync(BookingCreatedEvent e)
    {
        var response = await _httpClient.PostAsJsonAsync("api/notifications/booking-created", e);
        response.EnsureSuccessStatusCode();
    }
}
