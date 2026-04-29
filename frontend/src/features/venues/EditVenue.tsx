import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getAmenities, getVenueById, updateVenue } from "./api";
import type { Amenity, UpdateVenueRequest } from "./types";
import { getCurrentUserId } from "../auth/useAuth";

export default function EditVenuePage() {
  const navigate = useNavigate();
  const { id } = useParams();

  const [form, setForm] = useState<UpdateVenueRequest>({
    title: "",
    description: "",
    city: "",
    country: "",
    address: "",
    pricePerNight: 1,
    maxGuests: 1,
    bedrooms: 1,
    bathrooms: 1,
    amenityIds: []
  });

  const [amenities, setAmenities] = useState<Amenity[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadVenue() {
      if (!id) return;

      try {
        setLoading(true);
        setError("");

        const [venue, amenitiesData] = await Promise.all([
          getVenueById(id),
          getAmenities()
        ]);

        if (venue.ownerId !== getCurrentUserId()) {
          setError("You can edit only venues that you own.");
          return;
        }

        setAmenities(amenitiesData);
        setForm({
          title: venue.title,
          description: venue.description,
          city: venue.city,
          country: venue.country,
          address: venue.address,
          pricePerNight: venue.pricePerNight,
          maxGuests: venue.maxGuests,
          bedrooms: venue.bedrooms,
          bathrooms: venue.bathrooms,
          amenityIds: venue.amenities.map((amenity) => amenity.id)
        });
      } catch {
        setError("Failed to load venue.");
      } finally {
        setLoading(false);
      }
    }

    loadVenue();
  }, [id]);

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) {
    const { name, value } = e.target;
    const numberFields = [
      "pricePerNight",
      "maxGuests",
      "bedrooms",
      "bathrooms"
    ];

    setForm((prev) => ({
      ...prev,
      [name]: numberFields.includes(name) ? Number(value) : value
    }));
  }

  function handleAmenityToggle(amenityId: number) {
    setForm((prev) => {
      const alreadySelected = prev.amenityIds.includes(amenityId);

      return {
        ...prev,
        amenityIds: alreadySelected
          ? prev.amenityIds.filter((selectedId) => selectedId !== amenityId)
          : [...prev.amenityIds, amenityId]
      };
    });
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    if (!id) return;

    try {
      setSaving(true);
      setError("");
      await updateVenue(id, form);
      navigate(`/venues/${id}`);
    } catch {
      setError("Failed to update venue.");
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="create-venue-page">
        <div className="create-venue-form">
          <h1>Loading venue...</h1>
        </div>
      </div>
    );
  }

  return (
    <div className="create-venue-page">
      <form onSubmit={handleSubmit} className="create-venue-form">
        <h1>Edit venue</h1>

        {error && <p className="error-message">{error}</p>}

        <div className="form-group">
          <label>Title</label>
          <input
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Description</label>
          <textarea
            name="description"
            value={form.description}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>City</label>
            <input
              type="text"
              name="city"
              value={form.city}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Country</label>
            <input
              type="text"
              name="country"
              value={form.country}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <label>Address</label>
          <input
            type="text"
            name="address"
            value={form.address}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label>Price per night</label>
          <input
            type="number"
            name="pricePerNight"
            value={form.pricePerNight}
            onChange={handleChange}
            min={1}
            required
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label>Max guests</label>
            <input
              type="number"
              name="maxGuests"
              value={form.maxGuests}
              onChange={handleChange}
              min={1}
              required
            />
          </div>

          <div className="form-group">
            <label>Bedrooms</label>
            <input
              type="number"
              name="bedrooms"
              value={form.bedrooms}
              onChange={handleChange}
              min={1}
              required
            />
          </div>

          <div className="form-group">
            <label>Bathrooms</label>
            <input
              type="number"
              name="bathrooms"
              value={form.bathrooms}
              onChange={handleChange}
              min={1}
              required
            />
          </div>
        </div>

        <div className="form-group">
          <label>Amenities</label>

          <div className="amenities-list">
            {amenities.map((amenity) => (
              <label key={amenity.id} className="amenity-checkbox">
                <input
                  type="checkbox"
                  checked={form.amenityIds.includes(amenity.id)}
                  onChange={() => handleAmenityToggle(amenity.id)}
                />

                <span>{amenity.name}</span>
              </label>
            ))}
          </div>
        </div>

        <div className="form-actions">
          <button type="submit" disabled={saving || Boolean(error)}>
            {saving ? "Saving..." : "Save changes"}
          </button>

          <button
            type="button"
            className="secondary-button"
            onClick={() => navigate(id ? `/venues/${id}` : "/venues")}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
