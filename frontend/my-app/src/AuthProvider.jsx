import { useCallback, useEffect, useMemo, useState } from 'react';
import { AuthContext } from './auth-context';
import { apiFetch, clearToken, decodeToken, getToken, setToken } from './api';

// Loads the stored session and drops tokens that have already expired.
function readSession() {
  const token = getToken();
  const claims = token ? decodeToken(token) : null;

  if (!claims || (claims.exp && claims.exp * 1000 <= Date.now())) {
    clearToken();
    return null;
  }

  return claims;
}

export default function AuthProvider({ children }) {
  const [session, setSession] = useState(readSession);

  const logout = useCallback(() => {
    clearToken();
    setSession(null);
  }, []);

  const applyToken = useCallback((token) => {
    setToken(token);
    setSession(decodeToken(token));
  }, []);

  const login = useCallback(
    async (email, password) => {
      const data = await apiFetch('auth/login', {
        method: 'POST',
        auth: false,
        body: { email, password },
      });
      applyToken(data.token);
      return data;
    },
    [applyToken]
  );

  const register = useCallback(
    async ({ name, email, password }) => {
      const data = await apiFetch('auth/register', {
        method: 'POST',
        auth: false,
        body: { name, email, password },
      });
      // The backend currently returns a token too, so signing up also signs you in.
      if (data?.token) applyToken(data.token);
      return data;
    },
    [applyToken]
  );

  // A 401 on any request means the token was rejected.
  useEffect(() => {
    const handleExpired = () => setSession(null);
    window.addEventListener('auth:expired', handleExpired);
    return () => window.removeEventListener('auth:expired', handleExpired);
  }, []);

  // Sign out automatically when the token's lifetime runs out.
  useEffect(() => {
    if (!session?.exp) return undefined;

    const remaining = session.exp * 1000 - Date.now();
    const timer = setTimeout(logout, Math.max(remaining, 0));
    return () => clearTimeout(timer);
  }, [session, logout]);

  const value = useMemo(
    () => ({
      isLoggedIn: Boolean(session),
      memberId: session?.memberId ?? '',
      email: session?.email ?? '',
      role: session?.role ?? '',
      isAdmin: session?.role === 'Admin',
      login,
      register,
      logout,
    }),
    [session, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
