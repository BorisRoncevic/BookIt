import type { Venue } from "./types";
import type { ReviewSummary } from "../reviews/types";

type Props = {
  venue: Venue;
  summary?: ReviewSummary | undefined;
  onClick: () => void;
};

export default function VenueCard({ venue, summary, onClick }: Props) {
  const reviewCount = summary?.reviewCount ?? 0;

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
          <span>
            {summary && reviewCount > 0
              ? `${summary.averageRating.toFixed(1)} / 5`
              : "No reviews"}
          </span>
          <strong>{venue.pricePerNight} EUR / night</strong>
        </div>
      </div>
    </button>
  );
}
