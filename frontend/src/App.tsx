import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import "./App.css";

import Navbar from "./shared/components/Navbar";
import LoginPage from "./features/auth/LoginPage";
import RegisterPage from "./features/auth/RegisterPage";
import MyBookingsPage from "./features/bookings/MyBookings";
import CreateVenuePage from "./features/venues/CreateVenue";
import EditVenuePage from "./features/venues/EditVenue";
import VenueDetails from "./features/venues/VenueDetails";
import VenueList from "./features/venues/VenueList";

function App() {
  return (
    <BrowserRouter>
      <div className="app-shell">
        <Navbar />

        <main className="app-main">
          <Routes>
            <Route path="/" element={<VenueList />} />
            <Route path="/venues" element={<VenueList />} />
            <Route path="/venues/city/:city" element={<VenueList />} />
            <Route path="/venues/new" element={<CreateVenuePage />} />
            <Route path="/venues/:id/edit" element={<EditVenuePage />} />
            <Route path="/venues/:id" element={<VenueDetails />} />
            <Route path="/bookings" element={<MyBookingsPage />} />
            <Route path="/my-bookings" element={<Navigate to="/bookings" replace />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
