import { getToken,buildHeaders } from "../auth/useAuth";
import { BASE_URL } from "../auth/api";
import type { Venue,CreateVenueRequest } from "./types";


export async function createVenue(
  data: CreateVenueRequest
): Promise<Venue> {
  const token = getToken();

  const res = await fetch(`${BASE_URL}/venue`, {
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

export async function getVenues(): Promise<Venue[]> {
  const token = getToken(); // ako je endpoint public, možeš i bez tokena
  const res = await fetch(`${API_URL}/api/venues`, {
    method: "GET",
    headers: buildHeaders(token),
  });

  if (!res.ok) throw new Error("Failed to fetch venues");
  return res.json();
}

// GET /api/venues/{id}
export async function getVenueById(id: string): Promise<Venue> {
  const token = getToken();
  const res = await fetch(`${API_URL}/api/venues/${id}`, {
    method: "GET",
    headers: buildHeaders(token),
  });

  if (res.status === 404) throw new Error("Venue not found");
  if (!res.ok) throw new Error("Failed to fetch venue");

  return res.json();
}