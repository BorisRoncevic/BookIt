import { BASE_URL } from "../auth/api";
import { buildHeaders, getToken } from "../auth/useAuth";
import type { CreateReviewRequest, Review, ReviewSummary } from "./types";

export async function getVenueReviews(venueId: string): Promise<Review[]> {
  const res = await fetch(`${BASE_URL}/reviews/venue/${venueId}`);

  if (!res.ok) {
    throw new Error("Failed to load reviews");
  }

  return res.json();
}

export async function getReviewSummary(
  venueId: string
): Promise<ReviewSummary> {
  const res = await fetch(`${BASE_URL}/reviews/venue/${venueId}/summary`);

  if (!res.ok) {
    throw new Error("Failed to load review summary");
  }

  return res.json();
}

export async function createReview(
  data: CreateReviewRequest
): Promise<Review> {
  const token = getToken();
  if (!token) throw new Error("Unauthorized");

  const res = await fetch(`${BASE_URL}/reviews`, {
    method: "POST",
    headers: buildHeaders(token),
    body: JSON.stringify(data)
  });

  if (res.status === 409) {
    throw new Error("You already reviewed this venue.");
  }

  if (res.status === 401) {
    throw new Error("Please log in to leave a review.");
  }

  if (!res.ok) {
    throw new Error("Failed to create review");
  }

  return res.json();
}
