import { getToken,buildHeaders } from "../auth/useAuth";
import { BASE_URL } from "../auth/api";
import type { Venue,CreateVenueRequest,UpdateVenueRequest, Amenity} from "./types";


export async function createVenue(
  data: CreateVenueRequest
): Promise<Venue> {
  const token = getToken();

  const res = await fetch(`${BASE_URL}/venues`, {
    method : "POST",
    headers : buildHeaders(token),
    body : JSON.stringify(data)

  });

  if (res.status === 401) {
    throw new Error("Unauthorized");
  }

  if (!res.ok) {
    throw new Error("Failed to create booking");
  }

  return res.json();
}

export async function getAmenities(): Promise<Amenity[]> {
  const res = await fetch(`${BASE_URL}/venues/amenities`);

  if (!res.ok) {
    throw new Error("Failed to load amenities");
  }

  return res.json();
}

export async function getVenues(): Promise<Venue[]> {
  const token = getToken(); 
  const res = await fetch(`${BASE_URL}/venues`, {
    method: "GET",
    headers: buildHeaders(token),
  });

  if (!res.ok) throw new Error("Failed to fetch venues");
  return res.json();
}

export async function getVenueById(id: string): Promise<Venue> {
  const token = getToken();
  const res = await fetch(`${BASE_URL}/venues/${id}`, {
    method: "GET",
    headers: buildHeaders(token),
  });

  if (res.status === 404) throw new Error("Venue not found");
  if (!res.ok) throw new Error("Failed to fetch venue");

  return res.json();
}

export async function updateVenue(
  id: string,
  data: UpdateVenueRequest
): Promise<void> {
  const token = getToken();
  if (!token) throw new Error("Unauthorized");

  const res = await fetch(`${BASE_URL}/venues/${id}`, {
    method: "PUT",
    headers: buildHeaders(token),
    body: JSON.stringify(data),
  });

  if (res.status === 404) throw new Error("Venue not found");
  if (res.status === 401) throw new Error("Unauthorized");
  if (res.status === 403) throw new Error("Forbidden");
  if (!res.ok) throw new Error("Failed to update venue");
}

export async function deleteVenue(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error("Unauthorized");

  const res = await fetch(`${BASE_URL}/venues/${id}`, {
    method: "DELETE",
    headers: buildHeaders(token),
  });

  if (res.status === 404) throw new Error("Venue not found");
  if (res.status === 401) throw new Error("Unauthorized");
  if (res.status === 403) throw new Error("Forbidden");
  if (!res.ok) throw new Error("Failed to delete venue");
}
