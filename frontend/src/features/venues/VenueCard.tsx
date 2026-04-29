import type { Venue } from "./types";

type Props = {
  venue: Venue;
  onClick: () => void;
};

export default function VenueCard({ venue, onClick }: Props) {
  return (
    <button type="button" className="venue-card" onClick={onClick}>
      <div className="venue-card-image" aria-hidden="true">
        <span>{venue.city.slice(0, 1).toUpperCase()}</span>
      </div>

      <div className="venue-card-body">
        <div>
          <h2>{venue.title}</h2>
          <p>
            {venue.city}, {venue.country}
          </p>
        </div>

        <div className="venue-card-meta">
          <span>{venue.maxGuests} guests</span>
          <strong>{venue.pricePerNight} EUR / night</strong>
        </div>
      </div>
    </button>
  );
}
