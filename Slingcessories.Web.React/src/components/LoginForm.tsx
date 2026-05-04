import { FormEvent, useState } from 'react';
import { authApi } from '../services/api';

export default function LoginForm({ onLoggedIn }: { onLoggedIn: () => void }) {
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
      const ok = await authApi.login(email, password);
      if (!ok) {
        setError('Invalid email or password.');
        return;
      }
      onLoggedIn();
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
    <div className="accessories-list">
      <div className="header">
        <h1>Login</h1>
      </div>

      {error && <div className="error">Error: {error}</div>}
      {message && <div>{message}</div>}

      <form onSubmit={submitLogin}>
        <div>
          <label>Email</label>
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </div>
        <div>
          <label>Password</label>
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <div style={{ marginTop: 12, display: 'flex', gap: 8 }}>
          <button type="submit" disabled={busy}>Login</button>
          <button type="button" onClick={requestReset} disabled={busy}>Forgot Password</button>
        </div>
      </form>

      {showReset && (
        <div style={{ marginTop: 16 }}>
          <h3>Reset Password</h3>
          <div>
            <label>Reset Token</label>
            <input type="text" value={resetToken} onChange={(e) => setResetToken(e.target.value)} />
          </div>
          <div>
            <label>New Password</label>
            <input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
          </div>
          <button type="button" onClick={submitReset} disabled={busy}>Submit Reset</button>
        </div>
      )}
    </div>
  );
}
