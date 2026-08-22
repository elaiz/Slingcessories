import './NavMenu.css';

export type NavView = 'home' | 'accessories' | 'wishlist' | 'slingshots' | 'settings' | 'categories';

interface NavMenuProps {
  currentView: NavView;
  onNavigate: (view: NavView) => void;
}

export default function NavMenu({ currentView, onNavigate }: NavMenuProps) {
  return (
    <div className="nav-menu">
      <div className="nav-top-row">
        <span className="navbar-brand">Slingcessories</span>
      </div>
      <div className="nav-scrollable">
        <nav>
          <div className="nav-item">
            <button className={`nav-link${currentView === 'home' ? ' active' : ''}`} onClick={() => onNavigate('home')}>
              <span className="nav-icon nav-icon-home" aria-hidden="true"></span>
              Home
            </button>
          </div>
          <div className="nav-item">
            <button className={`nav-link${currentView === 'slingshots' ? ' active' : ''}`} onClick={() => onNavigate('slingshots')}>
              <span className="nav-icon nav-icon-slingshots" aria-hidden="true"></span>
              Slingshots
            </button>
          </div>
          <div className="nav-item">
            <button className={`nav-link${currentView === 'accessories' ? ' active' : ''}`} onClick={() => onNavigate('accessories')}>
              <span className="nav-icon nav-icon-accessories" aria-hidden="true"></span>
              Accessories
            </button>
          </div>
          <div className="nav-item">
            <button className={`nav-link${currentView === 'wishlist' ? ' active' : ''}`} onClick={() => onNavigate('wishlist')}>
              <span className="nav-icon nav-icon-wishlist" aria-hidden="true"></span>
              Wishlist
            </button>
          </div>
        </nav>
      </div>
    </div>
  );
}
