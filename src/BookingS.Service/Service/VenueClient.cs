using System.Net.Http.Json;

namespace BookingS.Service.Application.Services;

public class VenueClient : IVenueClient
{
    private readonly HttpClient _httpClient;

    public VenueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid?> GetVenueOwnerIdAsync(Guid venueId)
    {
        var venue = await _httpClient.GetFromJsonAsync<VenueOwnerResponse>(
            $"/api/venues/{venueId}");

        return venue?.OwnerId;
    }

    private sealed record VenueOwnerResponse(Guid OwnerId);
}
