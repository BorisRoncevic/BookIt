import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

import type { Venue } from "./types";
import { deleteVenue, getVenueById } from "./api";
import { getCurrentUserId } from "../auth/useAuth";

import type { BookedRange, Booking } from "../bookings/types";
import {
  cancelBooking,
  createBooking,
  getBookedRanges,
  getVenueBookings
} from "../bookings/api";

export default function VenueDetails() {
  const [venue, setVenue] = useState<Venue | null>(null);
  const [bookedRanges, setBookedRanges] = useState<BookedRange[]>([]);
  const [ownerBookings, setOwnerBookings] = useState<Booking[]>([]);

  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");

  const [loading, setLoading] = useState(true);
  const [bookingLoading, setBookingLoading] = useState(false);
  const [ownerBookingsLoading, setOwnerBookingsLoading] = useState(false);
  const [cancelingBookingId, setCancelingBookingId] = useState<string | null>(
    null
  );
  const [deleteLoading, setDeleteLoading] = useState(false);

  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const { id } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    async function loadVenueDetails() {
      if (!id) return;

      try {
        setLoading(true);
        setError("");

        const venueData = await getVenueById(id);
        const rangesData = await getBookedRanges(id);

        setVenue(venueData);
        setBookedRanges(rangesData);

        if (venueData.ownerId === getCurrentUserId()) {
          setOwnerBookingsLoading(true);
          const bookingsData = await getVenueBookings(id);
          setOwnerBookings(bookingsData);
        } else {
          setOwnerBookings([]);
        }
      } catch {
        setError("Failed to load venue details.");
      } finally {
        setOwnerBookingsLoading(false);
        setLoading(false);
      }
    }

    loadVenueDetails();
  }, [id]);

  function isRangeBooked(start: string, end: string) {
    return bookedRanges.some((range) => {
      return start < range.checkOut && end > range.checkIn;
    });
  }

  function getNights() {
    if (!checkIn || !checkOut) return 0;

    const start = new Date(checkIn);
    const end = new Date(checkOut);

    const diffInMs = end.getTime() - start.getTime();
    const nights = diffInMs / (1000 * 60 * 60 * 24);

    return nights > 0 ? nights : 0;
  }

  function getTotalPrice() {
    if (!venue) return 0;

    return getNights() * venue.pricePerNight;
  }

  function isBookingDisabled() {
    if (!venue) return true;
    if (!venue.isActive) return true;
    if (!checkIn || !checkOut) return true;
    if (checkIn >= checkOut) return true;
    if (isRangeBooked(checkIn, checkOut)) return true;

    return false;
  }

  async function handleBooking() {
    if (!venue) return;

    if (!checkIn || !checkOut) {
      setError("Please select check-in and check-out dates.");
      return;
    }

    if (checkIn >= checkOut) {
      setError("Check-out must be after check-in.");
      return;
    }

    if (isRangeBooked(checkIn, checkOut)) {
      setError("This venue is already booked in selected period.");
      return;
    }

    try {
      setBookingLoading(true);
      setError("");
      setSuccessMessage("");

      await createBooking({
        venueId: venue.id,
        checkIn,
        checkOut
      });

      const updatedRanges = await getBookedRanges(venue.id);
      setBookedRanges(updatedRanges);

      setCheckIn("");
      setCheckOut("");

      setSuccessMessage("Booking created successfully.");
    } catch {
      setError("Failed to create booking.");
    } finally {
      setBookingLoading(false);
    }
  }

  async function handleDeleteVenue() {
    if (!venue) return;

    const confirmed = window.confirm(
      "Are you sure you want to delete this venue?"
    );
    if (!confirmed) return;

    try {
      setDeleteLoading(true);
      setError("");
      await deleteVenue(venue.id);
      navigate("/venues");
    } catch {
      setError("Failed to delete venue.");
    } finally {
      setDeleteLoading(false);
    }
  }

  async function handleOwnerCancelBooking(bookingId: string) {
    if (!venue) return;

    const confirmed = window.confirm(
      "Are you sure you want to cancel this booking?"
    );
    if (!confirmed) return;

    try {
      setCancelingBookingId(bookingId);
      setError("");
      setSuccessMessage("");

      await cancelBooking(bookingId);

      setOwnerBookings((prev) =>
        prev.map((booking) =>
          booking.id === bookingId
            ? { ...booking, status: "Cancelled" }
            : booking
        )
      );

      const updatedRanges = await getBookedRanges(venue.id);
      setBookedRanges(updatedRanges);

      setSuccessMessage("Booking cancelled successfully.");
    } catch {
      setError("Failed to cancel booking.");
    } finally {
      setCancelingBookingId(null);
    }
  }

  const today = new Date().toISOString().slice(0, 10);
  const isOwner = venue?.ownerId === getCurrentUserId();
  const canCancelOwnerBooking = (booking: Booking) => {
    const checkOutDate = booking.checkOut.slice(0, 10);
    return booking.status === "Confirmed" && checkOutDate >= today;
  };

  if (loading) {
    return (
      <div className="venue-details-page">
        <h2>Loading venue...</h2>
      </div>
    );
  }

  if (!venue) {
    return (
      <div className="venue-details-page">
        <h2>Venue not found.</h2>
      </div>
    );
  }

  return (
    <div className="venue-details-page">
      <div className="venue-header">
        <div>
          <h1>{venue.title}</h1>

          <p>
            {venue.address}, {venue.city}, {venue.country}
          </p>
        </div>

        {isOwner && (
          <div className="owner-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={() => navigate(`/venues/${venue.id}/edit`)}
            >
              Edit venue
            </button>

            <button
              type="button"
              className="danger-button"
              onClick={handleDeleteVenue}
              disabled={deleteLoading}
            >
              {deleteLoading ? "Deleting..." : "Delete venue"}
            </button>
          </div>
        )}
      </div>

      <div className="venue-images">
        {venue.images?.length > 0 ? (
          venue.images.map((image) => (
            <img
              key={image.id}
              src={image.url}
              alt={venue.title}
              className="venue-image"
            />
          ))
        ) : (
          <p>No images available.</p>
        )}
      </div>

      <div className="venue-main-info">
        <div className="venue-description">
          <h2>Description</h2>
          <p>{venue.description}</p>

          <div className="venue-stats">
            <div>
              <strong>{venue.maxGuests}</strong>
              <span> Guests</span>
            </div>

            <div>
              <strong>{venue.bedrooms}</strong>
              <span> Bedrooms</span>
            </div>

            <div>
              <strong>{venue.bathrooms}</strong>
              <span> Bathrooms</span>
            </div>
          </div>

          <div className="venue-amenities">
            <h2>Amenities</h2>

            {venue.amenities?.length > 0 ? (
              <ul>
                {venue.amenities.map((amenity) => (
                  <li key={amenity.id}>{amenity.name}</li>
                ))}
              </ul>
            ) : (
              <p>No amenities listed.</p>
            )}
          </div>

          <div className="venue-footer-info">
            <p>Status: {venue.isActive ? "Active" : "Inactive"}</p>

            <p>
              Created at:{" "}
              {new Date(venue.createdAt).toLocaleDateString()}
            </p>
          </div>

          {isOwner && (
            <div className="owner-bookings">
              <div className="owner-bookings-header">
                <div>
                  <h2>Guest bookings</h2>
                  <p>Reservations made for this venue.</p>
                </div>
              </div>

              {ownerBookingsLoading ? (
                <p>Loading bookings...</p>
              ) : ownerBookings.length === 0 ? (
                <p>No guest bookings yet.</p>
              ) : (
                <div className="owner-bookings-list">
                  {ownerBookings.map((booking) => (
                    <div key={booking.id} className="owner-booking-row">
                      <div>
                        <strong>
                          {new Date(booking.checkIn).toLocaleDateString()} -{" "}
                          {new Date(booking.checkOut).toLocaleDateString()}
                        </strong>

                        <p>
                          Guest ID: {booking.userId} · {booking.totalPrice} €
                        </p>
                      </div>

                      <div className="owner-booking-actions">
                        <span
                          className={
                            booking.status === "Confirmed"
                              ? "status-confirmed"
                              : "status-cancelled"
                          }
                        >
                          {booking.status}
                        </span>

                        {canCancelOwnerBooking(booking) && (
                          <button
                            type="button"
                            className="danger-button"
                            onClick={() => handleOwnerCancelBooking(booking.id)}
                            disabled={cancelingBookingId === booking.id}
                          >
                            {cancelingBookingId === booking.id
                              ? "Cancelling..."
                              : "Cancel"}
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>

        <div className="venue-price-card">
          <h2>{venue.pricePerNight} €</h2>
          <p>per night</p>

          {error && <p className="error-message">{error}</p>}
          {successMessage && (
            <p className="success-message">{successMessage}</p>
          )}

          <div className="form-group">
            <label>Check in</label>
            <input
              type="date"
              value={checkIn}
              min={today}
              onChange={(e) => {
                const selectedDate = e.target.value;

                setCheckIn(selectedDate);
                setSuccessMessage("");
                setError("");

                if (checkOut && selectedDate >= checkOut) {
                  setCheckOut("");
                }
              }}
            />
          </div>

          <div className="form-group">
            <label>Check out</label>
            <input
              type="date"
              value={checkOut}
              min={checkIn || today}
              onChange={(e) => {
                setCheckOut(e.target.value);
                setSuccessMessage("");
                setError("");
              }}
            />
          </div>

          {checkIn && checkOut && checkIn >= checkOut && (
            <p className="error-message">
              Check-out must be after check-in.
            </p>
          )}

          {checkIn && checkOut && isRangeBooked(checkIn, checkOut) && (
            <p className="error-message">
              This period is already booked.
            </p>
          )}

          {checkIn &&
            checkOut &&
            checkIn < checkOut &&
            !isRangeBooked(checkIn, checkOut) && (
              <div className="booking-summary">
                <p>
                  <strong>{venue.pricePerNight} €</strong> x {getNights()}{" "}
                  nights
                </p>

                <p>
                  <strong>Total price:</strong> {getTotalPrice()} €
                </p>
              </div>
            )}

          <button
            type="button"
            onClick={handleBooking}
            disabled={bookingLoading || isBookingDisabled()}
          >
            {bookingLoading ? "Booking..." : "Book now"}
          </button>

          <div className="booked-ranges">
            <h3>Unavailable periods</h3>

            {bookedRanges.length === 0 ? (
              <p>No booked periods yet.</p>
            ) : (
              <ul>
                {bookedRanges.map((range, index) => (
                  <li key={index}>
                    {new Date(range.checkIn).toLocaleDateString()} -{" "}
                    {new Date(range.checkOut).toLocaleDateString()}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
