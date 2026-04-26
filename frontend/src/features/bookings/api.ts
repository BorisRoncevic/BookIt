import { getToken,buildHeaders } from "../auth/useAuth";
import { BASE_URL } from "../auth/api";
import type { CreateBookingRequest,Booking, BookedRange } from "./types";



export async function createBooking(
  data: CreateBookingRequest
): Promise<Booking> {
  const token = getToken();

  const res = await fetch(`${BASE_URL}/bookings`, {
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

export async function cancelBooking(id: string): Promise<void> {
    const token = getToken();
 const res = await fetch(`${BASE_URL}/bookings/${id}/cancel`, {
    method : "POST",
    headers:buildHeaders(token)
 });
 if (!res.ok) {
    throw new Error("failed to cancel booking")
 }

}

export async function getMyBookings(): Promise<Booking[]> {
  const token = getToken();

  const res = await fetch(`${BASE_URL}/bookings/my`, {
    method: "GET",
    headers: buildHeaders(token)
  });

  if (res.status === 401) {
    throw new Error("Unauthorized");
  }

  if (!res.ok) {
    throw new Error("Failed to load bookings");
  }

  return res.json();
}

export async function getBookedRanges(venueId: string): Promise<BookedRange[]> {
  const res = await fetch(`${BASE_URL}/bookings/venue/${venueId}/booked-ranges`);

  if (!res.ok) {
    throw new Error("Failed to load booked ranges");
  }

  return res.json();
}