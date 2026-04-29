import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getVenues } from "./api";
import type { Venue } from "./types";
import { useParams } from "react-router-dom";
import VenueCard from "./VenueCard";

export default function VenueList() {
  const [venues, setVenues] = useState<Venue[]>([]);
  const navigate = useNavigate();
  const { city } = useParams();

  useEffect(() => {
    getVenues().then(setVenues);
  }, []);

  const visibleVenues = city
    ? venues.filter((venue) => venue.city.toLowerCase() === city.toLowerCase())
    : venues;

  return (
    <div className="venues-page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Explore stays</span>
          <h1>{city ? `Stays in ${city}` : "Available venues"}</h1>
        </div>

        <p>
          Browse apartments, houses, and short stays ready for your next trip.
        </p>
      </div>

      {visibleVenues.length === 0 ? (
        <div className="empty-state">
          <h2>No venues found</h2>
          <p>Try another city or create the first venue for this location.</p>
        </div>
      ) : (
        <div className="venues-grid">
          {visibleVenues.map((venue) => (
            <VenueCard
              key={venue.id}
              venue={venue}
              onClick={() => navigate(`/venues/${venue.id}`)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
