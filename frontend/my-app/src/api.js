const TOKEN_KEY = 'token';

// Error with an HTTP status so pages can tell 401 apart from everything else.
export class ApiError extends Error {
  constructor(status, message, data) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.data = data;
  }
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

function decodeBase64Url(value) {
  const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
  const bytes = Uint8Array.from(atob(padded), (char) => char.charCodeAt(0));
  return new TextDecoder().decode(bytes);
}

// The backend sends claims under full SOAP URIs, so accept both shapes.
export function decodeToken(token) {
  if (!token) return null;

  try {
    const [, payload] = token.split('.');
    if (!payload) return null;

    const claims = JSON.parse(decodeBase64Url(payload));
    return {
      memberId:
        claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
        claims.nameid ??
        '',
      email:
        claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ??
        claims.email ??
        '',
      role:
        claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
        claims.role ??
        '',
      exp: claims.exp ?? 0,
    };
  } catch {
    return null;
  }
}

// The API still answers in Croatian; keep the UI in English.
const MESSAGES = {
  'Korisnik s tim e-mailom već postoji.': 'An account with that email already exists.',
  'Neispravan e-mail ili lozinka.': 'Wrong email or password.',
  'Neispravan identitet korisnika.': "We couldn't identify your account. Please log in again.",
  'Član s tim e-mailom već postoji.': 'A member with that email already exists.',
  'Član nije pronađen.': "We couldn't find that member.",
  'Pretplata ne postoji.': "That plan doesn't exist.",
  'Pretplata nije pronađena.': "We couldn't find that plan.",
  'Pretplata je dodijeljena članovima i ne može se obrisati.':
    'This plan is still assigned to members, so it cannot be deleted.',
  'Trener nije pronađen.': "We couldn't find that coach.",
  'Trener je dodijeljen pretplatama i ne može se obrisati.':
    'This coach is still assigned to plans, so they cannot be deleted.',
  'Unesite ime za pretragu.': 'Enter a name to search for.',
  'Ime je obavezno.': 'Name is required.',
  'Ime je predugo.': 'That name is too long.',
  'E-mail je obavezan.': 'Email is required.',
  'E-mail nije ispravan.': "That email doesn't look right.",
  'E-mail je predug.': 'That email is too long.',
  'Lozinka je obavezna.': 'Password is required.',
  'Lozinka mora imati najmanje 8 znakova.': 'Your password needs at least 8 characters.',
  'Tjedni broj dolazaka mora biti između 1 i 7.': 'Weekly visits must be between 1 and 7.',
  'Trajanje mora biti između 1 i 520 tjedana.': 'Duration must be between 1 and 520 weeks.',
  'Cijena mora biti između 0 i 100000.': 'Price must be between 0 and 100,000.',
};

function translate(message) {
  const text = String(message).trim();
  return MESSAGES[text] ?? text;
}

function validationMessage(data) {
  if (data && typeof data === 'object' && data.errors) {
    for (const value of Object.values(data.errors)) {
      const first = Array.isArray(value) ? value.find(Boolean) : value;
      if (first) return translate(first);
    }
  }
  return '';
}

function errorMessage(status, data) {
  if (typeof data === 'string' && data.trim()) return translate(data);
  const validation = validationMessage(data);
  if (validation) return validation;
  if (data && typeof data === 'object' && typeof data.title === 'string') return data.title;

  switch (status) {
    case 400:
      return "Some of those details don't look right. Please check and try again.";
    case 401:
      return "You're not signed in, or your session has expired.";
    case 403:
      return "You don't have permission to do that.";
    case 404:
      return "We couldn't find what you were looking for.";
    case 409:
      return 'That record already exists.';
    default:
      return 'Something went wrong. Please try again.';
  }
}

/**
 * Calls the API. Paths are relative (e.g. "member/search?name=ana") and are
 * prefixed with /api, which the Vite proxy forwards to the backend.
 */
export async function apiFetch(path, { method = 'GET', body, auth = true } = {}) {
  const headers = {};
  if (body !== undefined) headers['Content-Type'] = 'application/json';

  const token = getToken();
  if (auth && token) headers.Authorization = `Bearer ${token}`;

  let response;
  try {
    response = await fetch(`/api/${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    throw new ApiError(0, "Can't reach the server. Is the API running?");
  }

  const raw = await response.text();
  let data = null;
  if (raw) {
    try {
      data = JSON.parse(raw);
    } catch {
      data = raw;
    }
  }

  if (!response.ok) {
    // Session expired – the provider listens for this event and signs the user out.
    if (response.status === 401 && auth && token) {
      window.dispatchEvent(new Event('auth:expired'));
    }
    throw new ApiError(response.status, errorMessage(response.status, data), data);
  }

  return data;
}
