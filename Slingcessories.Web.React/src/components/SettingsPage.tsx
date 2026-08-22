import { NavView } from './NavMenu';

interface Props {
  onNavigate: (view: NavView) => void;
}

export default function SettingsPage({ onNavigate }: Props) {
  return (
    <div>
      <h1>Settings</h1>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        <li style={{ marginBottom: '0.75rem' }}>
          <button className="btn-link" style={{ fontSize: '1rem' }} onClick={() => onNavigate('categories')}>
            Categories &amp; Subcategories
          </button>
        </li>
      </ul>
    </div>
  );
}
