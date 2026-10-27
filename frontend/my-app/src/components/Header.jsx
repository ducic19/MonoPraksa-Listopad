import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth-context';

export default function Header() {
  const { isLoggedIn, email, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/');
  }

  return (
    <header className="site-header">
      <div className="container header-inner">
        <Link to="/" className="brand">
          THE<span>GYM</span>
        </Link>

        <nav className="nav">
          <NavLink to="/" end className={({ isActive }) => (isActive ? 'active' : '')}>
            Home
          </NavLink>
          <NavLink to="/search" className={({ isActive }) => (isActive ? 'active' : '')}>
            Members
          </NavLink>
        </nav>

        {isLoggedIn ? (
          <div className="user">
            <span>{email}</span>
            <button type="button" className="btn btn-sm" onClick={handleLogout}>
              Log out
            </button>
          </div>
        ) : (
          <div className="user">
            <Link to="/login" className="btn btn-primary btn-sm">
              Log in
            </Link>
          </div>
        )}
      </div>
    </header>
  );
}
