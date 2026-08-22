import { FormEvent, useState } from 'react';
import { authApi, UserInfo } from '../services/api';
import './LoginForm.css';

interface Props {
  onRegistered: (user: UserInfo) => void;
  onBackToLogin: () => void;
}

export default function RegisterForm({ onRegistered, onBackToLogin }: Props) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const user = await authApi.register({
        firstName,
        lastName,
        email,
        password,
        clientId: 'Slingcessories.React',
      });
      if (user) onRegistered(user);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1 className="login-title">Slingcessories</h1>
        <h2 className="login-subtitle">Create an account</h2>

        {error && <div className="login-alert login-alert-error">{error}</div>}

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="firstName">First Name</label>
            <input id="firstName" type="text" value={firstName} onChange={e => setFirstName(e.target.value)} required />
          </div>
          <div className="form-group">
            <label htmlFor="lastName">Last Name</label>
            <input id="lastName" type="text" value={lastName} onChange={e => setLastName(e.target.value)} required />
          </div>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input id="email" type="email" value={email} onChange={e => setEmail(e.target.value)} required autoComplete="email" />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input id="password" type="password" value={password} onChange={e => setPassword(e.target.value)} required autoComplete="new-password" />
          </div>
          <div className="login-actions">
            <button type="submit" className="btn-primary" disabled={busy}>
              {busy ? 'Registering…' : 'Register'}
            </button>
          </div>
        </form>

        <div className="login-register-link">
          Already have an account?{' '}
          <button type="button" className="btn-link-small" onClick={onBackToLogin}>
            Log in
          </button>
        </div>
      </div>
    </div>
  );
}
