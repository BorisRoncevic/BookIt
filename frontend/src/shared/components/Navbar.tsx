import { NavLink, useNavigate } from "react-router-dom";
import { getToken } from "../../features/auth/useAuth";

export default function Navbar() {
  const navigate = useNavigate();
  const isLoggedIn = Boolean(getToken());

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    navigate("/login");
  }

  return (
    <header className="navbar">
      <NavLink to="/" className="brand">
        AirbnbBooking
      </NavLink>

      <nav className="nav-links">
        <NavLink to="/venues">Venues</NavLink>
        <NavLink to="/venues/new">Create venue</NavLink>
        <NavLink to="/bookings">My bookings</NavLink>
      </nav>

      <div className="nav-actions">
        {isLoggedIn ? (
          <button type="button" onClick={handleLogout}>
            Logout
          </button>
        ) : (
          <>
            <NavLink to="/login">Login</NavLink>
            <NavLink to="/register" className="primary-link">
              Register
            </NavLink>
          </>
        )}
      </div>
    </header>
  );
}
