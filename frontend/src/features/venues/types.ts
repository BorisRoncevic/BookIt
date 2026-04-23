
export type Amenity = {
  id: number;
  name: string;
};

export type VenueImage = {
  id: string;
  url: string;
};

export type Venue = {
  id: string;

  ownerId: string;

  title: string;
  description: string;

  city: string;
  country: string;
  address: string;

  pricePerNight: number;

  maxGuests: number;
  bedrooms: number;
  bathrooms: number;

  isActive: boolean;

  createdAt: string;

  images: VenueImage[];
  amenities: Amenity[];
};


export type CreateVenueRequest = {
  title: string;
  description: string;

  city: string;
  country: string;
  address: string;

  pricePerNight: number;

  maxGuests: number;
  bedrooms: number;
  bathrooms: number;

  amenities: number[]; // ID-evi
};