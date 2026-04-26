import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import type { Venue } from "./types";
import { getVenueById } from "./api";

import type { BookedRange } from "../bookings/types";
import { createBooking, getBookedRanges } from "../bookings/api";

export default function VenueDetails() {
  const [venue, setVenue] = useState<Venue | null>(null);
  const [bookedRanges, setBookedRanges] = useState<BookedRange[]>([]);

  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");

  const [loading, setLoading] = useState(true);
  const [bookingLoading, setBookingLoading] = useState(false);

  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const { id } = useParams();

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
      } catch {
        setError("Failed to load venue details.");
      } finally {
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

  const today = new Date().toISOString().split("T")[0];

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
        <h1>{venue.title}</h1>

        <p>
          {venue.address}, {venue.city}, {venue.country}
        </p>
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