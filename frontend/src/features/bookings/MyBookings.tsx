import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { Booking } from "./types";
import { getMyBookings, cancelBooking } from "./api";

export default function MyBookingsPage() {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [cancelingId, setCancelingId] = useState<string | null>(null);

  const navigate = useNavigate();

  
useEffect(() => {
  async function loadBookings() {
    try {
      setLoading(true);
      setError("");

      const data = await getMyBookings();
      setBookings(data);
    } catch {
      setError("Failed to load bookings.");
    } finally {
      setLoading(false);
    }
  }

  loadBookings();
}, []);

  async function handleCancel(id: string) {
    try {
      setCancelingId(id);
      setError("");

      await cancelBooking(id);

      setBookings((prev) =>
        prev.map((booking) =>
          booking.id === id
            ? { ...booking, status: "Cancelled" }
            : booking
        )
      );
    } catch  {
      setError("Failed to cancel booking.");
    } finally {
      setCancelingId(null);
    }
  }

  if (loading) {
    return (
      <div className="my-bookings-page">
        <h2>Loading bookings...</h2>
      </div>
    );
  }

  return (
    <div className="my-bookings-page">
      <div className="my-bookings-header">
        <h1>My bookings</h1>
        <p>All venues you have reserved.</p>
      </div>

      {error && <p className="error-message">{error}</p>}

      {bookings.length === 0 ? (
        <div className="empty-bookings">
          <h2>No bookings yet</h2>
          <p>You have not reserved any venues yet.</p>

          <button onClick={() => navigate("/")}>
            Browse venues
          </button>
        </div>
      ) : (
        <div className="bookings-list">
          {bookings.map((booking) => (
            <div key={booking.id} className="booking-card">
              <div className="booking-info">
                <h2>Booking #{booking.id}</h2>

                <p>
                  <strong>Venue ID:</strong> {booking.venueId}
                </p>

                <p>
                  <strong>Check in:</strong>{" "}
                  {new Date(booking.checkIn).toLocaleDateString()}
                </p>

                <p>
                  <strong>Check out:</strong>{" "}
                  {new Date(booking.checkOut).toLocaleDateString()}
                </p>

                <p>
                  <strong>Total price:</strong> {booking.totalPrice} €
                </p>

                <p>
                  <strong>Status:</strong>{" "}
                  <span
                    className={
                      booking.status === "Confirmed"
                        ? "status-confirmed"
                        : "status-cancelled"
                    }
                  >
                    {booking.status}
                  </span>
                </p>

                <p>
                  <strong>Created at:</strong>{" "}
                  {new Date(booking.createdAt).toLocaleDateString()}
                </p>
              </div>

              <div className="booking-actions">
                <button onClick={() => navigate(`/venues/${booking.venueId}`)}>
                  View venue
                </button>

                {booking.status === "Confirmed" && (
                  <button
                    onClick={() => handleCancel(booking.id)}
                    disabled={cancelingId === booking.id}
                    className="cancel-booking-button"
                  >
                    {cancelingId === booking.id
                      ? "Canceling..."
                      : "Cancel booking"}
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}