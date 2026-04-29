import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createVenue, getAmenities } from "./api";
import type { Amenity, CreateVenueRequest } from "./types";

export default function CreateVenuePage() {
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateVenueRequest>({
    title: "",
    description: "",
    city: "",
    country: "",
    address: "",
    pricePerNight: 0,
    maxGuests: 1,
    bedrooms: 1,
    bathrooms: 1,
    amenityIds: []
  });

  const [amenities, setAmenities] = useState<Amenity[]>([]);
  const [loading, setLoading] = useState(false);
  const [amenitiesLoading, setAmenitiesLoading] = useState(true);
  const [amenitiesError, setAmenitiesError] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadAmenities() {
      try {
        setAmenitiesError("");
        const data = await getAmenities();
        setAmenities(data);
      } catch {
        setAmenitiesError("Failed to load amenities.");
      } finally {
        setAmenitiesLoading(false);
      }
    }

    loadAmenities();
  }, []);

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
          ? prev.amenityIds.filter((id) => id !== amenityId)
          : [...prev.amenityIds, amenityId]
      };
    });
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();

    try {
      setLoading(true);
      setError("");

      const createdVenue = await createVenue(form);

      navigate(`/venues/${createdVenue.id}`);
    } catch {
      setError("Failed to create venue.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="create-venue-page">
      <form onSubmit={handleSubmit} className="create-venue-form">
        <h1>Create venue</h1>

        {error && <p className="error-message">{error}</p>}

        <div className="form-group">
          <label>Title</label>
          <input
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
            placeholder="Modern apartment"
            required
          />
        </div>

        <div className="form-group">
          <label>Description</label>
          <textarea
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Describe your venue..."
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
              placeholder="Belgrade"
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
              placeholder="Serbia"
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
            placeholder="Knez Mihailova 10"
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

          {amenitiesLoading ? (
            <p>Loading amenities...</p>
          ) : amenitiesError ? (
            <p className="error-message">{amenitiesError}</p>
          ) : (
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
          )}
        </div>

        <button type="submit" disabled={loading}>
          {loading ? "Creating..." : "Create venue"}
        </button>
      </form>
    </div>
  );
}
