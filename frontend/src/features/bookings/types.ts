export type CreateBookingRequest = {
  venueId: string;
  checkIn: string;   
  checkOut: string;  
};
export type BookingStatus = "Confirmed" | "Cancelled";

export type Booking = {
  id: string;
  venueId: string;
  userId: string;

  checkIn: string;    
  checkOut: string;

  totalPrice: number;

  status: BookingStatus;

  createdAt: string;  
};