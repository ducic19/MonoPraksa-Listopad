import { createContext, useContext } from 'react';

export const AuthContext = createContext(null);

// Kept apart from the provider so react-refresh doesn't complain about mixed exports.
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used inside an AuthProvider.');
  }
  return context;
}
