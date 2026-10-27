import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import { useAuth } from './auth-context';

export default function Login() {
  const { isLoggedIn, login, register } = useAuth();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const [mode, setMode] = useState(params.get('mode') === 'register' ? 'register' : 'login');
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (isLoggedIn) navigate('/', { replace: true });
  }, [isLoggedIn, navigate]);

  function switchMode(nextMode) {
    setMode(nextMode);
    setError('');
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setBusy(true);
    setError('');

    try {
      if (mode === 'login') {
        await login(email, password);
      } else {
        await register({ name, email, password });
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="app">
      <Header />
      <main className="page">
        <div className="auth container">
          <div className="card">
            <div className="tabs">
              <button
                type="button"
                className={mode === 'login' ? 'tab active' : 'tab'}
                onClick={() => switchMode('login')}
              >
                Log in
              </button>
              <button
                type="button"
                className={mode === 'register' ? 'tab active' : 'tab'}
                onClick={() => switchMode('register')}
              >
                Sign up
              </button>
            </div>

            <h1>{mode === 'login' ? 'Welcome back' : 'Create your account'}</h1>

            {mode === 'register' && (
              <p className="auth-note">The hardest part is showing up — you're a minute away.</p>
            )}

            <form className="form" onSubmit={handleSubmit}>
              {mode === 'register' && (
                <div className="field">
                  <label htmlFor="name">Full name</label>
                  <input
                    id="name"
                    className="input"
                    type="text"
                    autoComplete="name"
                    required
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                  />
                </div>
              )}

              <div className="field">
                <label htmlFor="email">Email</label>
                <input
                  id="email"
                  className="input"
                  type="email"
                  autoComplete="email"
                  required
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                />
              </div>

              <div className="field">
                <label htmlFor="password">Password</label>
                <input
                  id="password"
                  className="input"
                  type="password"
                  autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
                  minLength={mode === 'register' ? 8 : undefined}
                  required
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                />
              </div>

              {error && (
                <p className="alert alert-error" role="alert">
                  {error}
                </p>
              )}
              <button type="submit" className="btn btn-primary btn-block" disabled={busy}>
                {busy ? 'One moment…' : mode === 'login' ? 'Log in' : 'Create account'}
              </button>
            </form>

            <p className="auth-hint">
              Demo login: luka@gmail.com / 12345678
            </p>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}
