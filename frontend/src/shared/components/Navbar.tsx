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
    <nav className="navbar">
      <NavLink to="/venues" className="brand">
        BookIt
      </NavLink>

      <div className="nav-links">
        <NavLink to="/venues">Venues</NavLink>
        {isLoggedIn && <NavLink to="/bookings">My bookings</NavLink>}
        {isLoggedIn && (
          <NavLink to="/venues/new" className="primary-link">
            Add venue
          </NavLink>
        )}
      </div>

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
    </nav>
  );
}
