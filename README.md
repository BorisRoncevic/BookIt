# Airbnb Booking Application

## Overview

This project is a full-stack web application for booking accommodation, inspired by platforms such as Airbnb and Booking.com. It allows users to browse apartments, view details, select dates, and create bookings, while owners can create and manage accommodation listings.

The main focus of the project is realistic booking availability, where the system prevents users from booking a venue during an already reserved period.

## Core Features

### Authentication

- User registration and login
- JWT-based authentication
- Protected backend endpoints
- User identity extracted from the token
- Role-based behavior for customers and owners

### Venue Management

Users can browse accommodation listings and view detailed information about each venue.

Each venue contains:

- title and description
- city, country, and address
- price per night
- maximum number of guests
- bedrooms and bathrooms
- images
- amenities
- active status

### Owner Functionality

Authenticated owners can create new accommodation listings.

When creating a venue, owners can enter basic property information and select available amenities from a predefined list.

Example amenities:

- WiFi
- Parking
- Kitchen
- Air conditioning
- TV

### Booking System

Authenticated users can create bookings by selecting a check-in and check-out date.

The total price is calculated based on:

- number of nights
- venue price per night

