namespace Venue.Service.Application.Interfaces;
using Venue.Service.Models;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id);
    Task<List<Venue>> GetAllAsync();

    Task AddAsync(Venue venue);
    Task UpdateAsync(Venue venue);
    Task DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
}