namespace BookingS.Service.Model;

public enum CancelBookingResult
{
    Cancelled,
    NotFound,
    Forbidden,
    AlreadyCancelled,
    PastBooking
}
