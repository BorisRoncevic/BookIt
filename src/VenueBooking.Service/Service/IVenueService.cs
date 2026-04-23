namespace VenueBooking.Service.Application.Interfaces;

using VenueBooking.Service.Models;

public interface IVenueService
{
    Task<List<Venue>> GetAllAsync();
    Task<Venue?> GetByIdAsync(Guid id);
    Task<Venue> CreateAsync(Venue venue);
    Task<bool> UpdateAsync(Guid id, Venue venue);
    Task<bool> DeleteAsync(Guid id);
}