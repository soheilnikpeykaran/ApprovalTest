import { FormEvent, useState } from 'react';
import { api } from '../services/api';
import type { AuthResponse } from '../types';

export function Login({
  onLogin,
}: {
  onLogin: (auth: AuthResponse) => void;
}) {
  const [email, setEmail] = useState('employee@test.com');
  const [password, setPassword] = useState('Employee123!');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function submit(e: FormEvent) {
    e.preventDefault();

    setError('');
    setLoading(true);

    try {
      const auth = await api.login(email, password);

      localStorage.setItem('access_token', auth.token);
      localStorage.setItem('roles', JSON.stringify(auth.roles));

      onLogin(auth);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'خطا در ورود به سامانه'
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="card auth">
      <h1>سامانه گردش درخواست</h1>

      <p className="subtitle">
        برای ورود، اطلاعات حساب کاربری خود را وارد کنید.
      </p>

      <form onSubmit={submit}>
        <label>
          ایمیل
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            type="email"
            required
          />
        </label>

        <label>
          رمز عبور
          <input
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            type="password"
            required
          />
        </label>

        {error && (
          <p className="error">
            {error}
          </p>
        )}

        <button disabled={loading}>
          {loading ? 'در حال ورود...' : 'ورود'}
        </button>
      </form>

      <p className="hint">
        حساب تست: employee@test.com / Employee123!
      </p>
    </div>
  );
}