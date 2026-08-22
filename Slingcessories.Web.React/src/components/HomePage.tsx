import { NavView } from './NavMenu';
import './HomePage.css';

interface Props {
  onNavigate: (view: NavView) => void;
}

export default function HomePage({ onNavigate }: Props) {
  return (
    <div className="home-page">
      <h1>Slingcessories!</h1>
      <p className="home-tagline">A place where you can keep track of your Slingshot stuff.</p>

      <h3 className="home-section-title">Quick Navigation</h3>
      <div className="home-cards">
        <div className="home-card">
          <h5>🔧 Accessories</h5>
          <p>Manage your slingshot accessories collection.</p>
          <button className="btn-primary" onClick={() => onNavigate('accessories')}>
            View Accessories
          </button>
        </div>
        <div className="home-card">
          <h5>🎯 Slingshots</h5>
          <p>Keep track of your slingshot models.</p>
          <button className="btn-primary" onClick={() => onNavigate('slingshots')}>
            View Slingshots
          </button>
        </div>
        <div className="home-card">
          <h5>⭐ Wishlist</h5>
          <p>Browse accessories you want to get.</p>
          <button className="btn-primary" onClick={() => onNavigate('wishlist')}>
            View Wishlist
          </button>
        </div>
      </div>
    </div>
  );
}
