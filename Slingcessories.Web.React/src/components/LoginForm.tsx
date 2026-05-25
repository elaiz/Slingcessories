import { FormEvent, useState } from 'react';
import { authApi, UserInfo } from '../services/api';
import './LoginForm.css';

interface Props {
  onLoggedIn: (user: UserInfo) => void;
}

export default function LoginForm({ onLoggedIn }: Props) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [resetToken, setResetToken] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [showReset, setShowReset] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const submitLogin = async (e: FormEvent) => {
    e.preventDefault();
    setBusy(true);
    setError(null);
    setMessage(null);

    try {
      const user = await authApi.login(email, password);
      if (!user) {
        setError('Invalid email or password.');
        return;
      }
      onLoggedIn(user);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
    } finally {
      setBusy(false);
    }
  };

  const requestReset = async () => {
    if (!email.trim()) {
      setError('Enter your email first.');
      return;
    }

    setBusy(true);
    setError(null);
    setMessage(null);

    try {
      const token = await authApi.forgotPassword(email);
      setShowReset(true);
      setResetToken(token ?? '');
      setMessage(token
        ? 'Development reset token generated.'
        : 'If the account exists, reset instructions were generated.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Forgot password failed');
    } finally {
      setBusy(false);
    }
  };

  const submitReset = async () => {
    if (!resetToken.trim() || !newPassword.trim()) {
      setError('Reset token and new password are required.');
      return;
    }

    setBusy(true);
    setError(null);
    setMessage(null);

    try {
      const ok = await authApi.resetPassword(email, resetToken, newPassword);
      setMessage(ok ? 'Password reset successful.' : 'Reset failed.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Reset password failed');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1 className="login-title">Slingcessories</h1>
        <h2 className="login-subtitle">Sign in</h2>

        {error && <div className="login-alert login-alert-error">{error}</div>}
        {message && <div className="login-alert login-alert-info">{message}</div>}

        <form onSubmit={submitLogin} className="login-form">
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required autoComplete="email" />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required autoComplete="current-password" />
          </div>
          <div className="login-actions">
            <button type="submit" className="btn-primary" disabled={busy}>
              {busy ? 'Signing in…' : 'Log in'}
            </button>
            <button type="button" className="btn-link-small" onClick={requestReset} disabled={busy}>
              Forgot password?
            </button>
          </div>
        </form>

        {showReset && (
          <div className="reset-section">
            <h3>Reset Password</h3>
            <div className="form-group">
              <label htmlFor="resetToken">Reset Token</label>
              <input id="resetToken" type="text" value={resetToken} onChange={(e) => setResetToken(e.target.value)} />
            </div>
            <div className="form-group">
              <label htmlFor="newPassword">New Password</label>
              <input id="newPassword" type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
            </div>
            <button type="button" className="btn-primary" onClick={submitReset} disabled={busy}>
              Submit Reset
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
