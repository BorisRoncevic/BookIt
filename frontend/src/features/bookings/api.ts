import { getToken,buildHeaders } from "../auth/useAuth";
import { BASE_URL } from "../auth/api";
import type { CreateBookingRequest,Booking } from "./types";



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