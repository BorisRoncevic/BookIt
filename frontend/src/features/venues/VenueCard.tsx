import type { Venue } from "./types";

type Props = {
  venue: Venue;
  onClick: () => void;
};

export default function VenueCard({ venue, onClick }: Props) {
  return (
    <div className="venue-card" onClick={onClick}>
      <h3>{venue.title}</h3>
      <p>{venue.city}</p>
      <p>{venue.pricePerNight}€</p>
    </div>
  );
}