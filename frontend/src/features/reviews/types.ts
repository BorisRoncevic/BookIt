export type Review = {
  id: string;
  venueId: string;
  userId: string;
  rating: number;
  comment: string;
  createdAt: string;
};

export type ReviewSummary = {
  venueId: string;
  averageRating: number;
  reviewCount: number;
};

export type CreateReviewRequest = {
  venueId: string;
  rating: number;
  comment: string;
};
