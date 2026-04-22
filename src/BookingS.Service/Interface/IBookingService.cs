public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<List<Booking>> GetByVenueIdAsync(Guid venueId);
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<Booking?> GetByIdAsync(Guid id);
    Task UpdateAsync(Booking booking);
}