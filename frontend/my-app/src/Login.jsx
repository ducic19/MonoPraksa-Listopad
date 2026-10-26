// src/Login.jsx
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'

function Login() {
  const [mode, setMode] = useState('login') // 'login' ili 'register'
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const navigate = useNavigate()

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setBusy(true)
    try {
      const res = await fetch(`/api/auth/${mode}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(
          mode === 'register' ? { name, email, password } : { email, password }
        ),
      })
           if (!res.ok) {
        throw new Error(
          mode === 'login' ? 'Pogrešan e-mail ili lozinka.' : 'Registracija nije uspjela.'
        )
      }
      const data = await res.json().catch(() => ({}))
      if (data.token) localStorage.setItem('token', data.token)
      if (mode === 'register' && !data.token) {
        setMode('login')
        setError('Registracija uspjela. Sada se prijavi.')
        return
      }
      navigate('/')
    } catch (err) {
      setError(err.message)
    } finally {
      setBusy(false)
    }
  }

  return (
    <div id="glavni">
      <h1 className="naslov">THEGYM</h1>

      <div className="kartica">
        <div className="tabovi">
          <button
            type="button"
            className={mode === 'login' ? 'tab aktivan' : 'tab'}
            onClick={() => setMode('login')}
          >
            Prijava
          </button>
          <button
            type="button"
            className={mode === 'register' ? 'tab aktivan' : 'tab'}
            onClick={() => setMode('register')}
          >
            Registracija
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          {mode === 'register' && (
            <input
              className="polje"
              type="text"
              placeholder="Ime i prezime"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          )}
          <input
            className="polje"
            type="email"
            placeholder="E-mail"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input
            className="polje"
            type="password"
            placeholder="Lozinka"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          {error && <p className="greska">{error}</p>}

          <button className="gumb" type="submit" disabled={busy}>
            {busy ? 'Molim pričekaj…' : mode === 'login' ? 'Prijavi se' : 'Registriraj se'}
          </button>
        </form>

        <p><Link to="/">← Natrag na početnu</Link></p>
      </div>
    </div>
  )
}

export default Login