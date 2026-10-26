import { useState } from 'react'
import { Link } from 'react-router-dom'
import './App.css'

function Search() {
  const token = localStorage.getItem('token')
  const [searchTerm, setSearchTerm] = useState('')
  const [members, setMembers] = useState([])
  const [message, setMessage] = useState('')

  const handleSearch = async (e) => {
    e.preventDefault()
    setMessage('')
    try {
      const res = await fetch(`/api/member/search?name=${encodeURIComponent(searchTerm)}`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      if (res.status === 401) {
        setMessage('Sesija je istekla. Prijavi se ponovno.')
        return
      }
      if (!res.ok) {
        setMessage('Greška prilikom pretraživanja.')
        return
      }
      const data = await res.json()
      setMembers(data)
      if (data.length === 0) setMessage('Nema pronađenih članova.')
    } catch {
      setMessage('Server nije dostupan.')
    }
  }

  if (!token) {
    return (
      <div id="glavni">
        <h1 className="naslov">THEGYM</h1>
        <div className="kartica">
          <p>Molimo prijavite se za pristup pretraživanju članova.</p>
          <Link className="gumb" to="/login">Prijava</Link>
        </div>
      </div>
    )
  }

  return (
    <div id="glavni">
      <h1 className="naslov">THEGYM</h1>
      <div className="kartica">
        <h2>Pretraživanje članova</h2>
        <form onSubmit={handleSearch}>
          <input
            className="polje"
            type="text"
            placeholder="Upiši ime člana..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <button className="gumb" type="submit">Traži</button>
        </form>

        {message && <p>{message}</p>}

        <ul className="lista">
          {members.map((m, i) => (
            <li key={m.id ?? i}>
              <strong>{m.name || m.email}</strong> – Aktivna ponuda: {m.activeOffer || 'Nema'}
            </li>
          ))}
        </ul>

        <p><Link to="/">← Natrag na početnu</Link></p>
      </div>
    </div>
  )
}

export default Search