import React, { useState } from 'react';
import './App.css';

const offers = [
  { name: 'Dnevna karta', price: '6€' },
  { name: 'Mjesečna karta', price: '30€' },
  { name: 'Godišnja karta', price: '300€' }
];

function App() {
  // Provjeravamo u localStorage je li korisnik već prijavljen
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const [userEmail, setUserEmail] = useState(localStorage.getItem('userEmail') || '');
  const [activeOffer, setActiveOffer] = useState(localStorage.getItem('activeOffer') || '');
  
  // Stanja za formu prijave / registracije
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [selectedOffer, setSelectedOffer] = useState(offers[0].name);
  const [isRegisterMode, setIsRegisterMode] = useState(false);

  // Stanja za pretragu članova
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState([]);

  // 1. Prijava – spremanje tokena u localStorage i postavljanje statusa
  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch('api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });

      if (response.ok) {
        const data = await response.json();
        const jwtToken = data.token || data;
        const offer = data.selectedOffer || data.activeOffer || 'Mjesečna karta';

        // Spremamo u localStorage
        localStorage.setItem('token', jwtToken);
        localStorage.setItem('userEmail', email);
        localStorage.setItem('activeOffer', offer);

        // Ažuriramo React stanja da se odmah prebaci prikaz
        setUserEmail(email);
        setActiveOffer(offer);
        setIsLoggedIn(true);
      } else {
        alert('Neispravan e-mail ili lozinka.');
      }
    } catch (error) {
      console.error('Greška pri prijavi:', error);
    }
  };

  // 2. Registracija
  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch('api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          email: email, 
          password: password, 
          selectedOffer: selectedOffer 
        })
      });

      if (response.ok) {
        alert(`Uspješna registracija s odabranom ponudom: ${selectedOffer}! Sada se možete prijaviti.`);
        setIsRegisterMode(false);
      } else {
        const errText = await response.text();
        alert(errText || 'Greška pri registraciji.');
      }
    } catch (error) {
      console.error('Greška pri registraciji:', error);
    }
  };

  // 3. Pretraga članova – dohvaća token iz localStorage
  const handleSearch = async (e) => {
    e.preventDefault();
    const token = localStorage.getItem('token');
    
    try {
      const response = await fetch(`api/member/search?name=${searchTerm}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        const data = await response.json();
        setSearchResults(data);
      } else {
        alert('Greška pri pretraživanju članova.');
      }
    } catch (error) {
      console.error('Greška:', error);
    }
  };

  // Odjava – briše podatke iz localStorage
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('activeOffer');
    setIsLoggedIn(false);
    setUserEmail('');
    setActiveOffer('');
    setSearchResults([]);
  };

  return (
    <div id="glavni">
      <h1 className="naslov">THEGYM</h1>
      <p className="podnaslov">nova i vjerojatno najpraznija teretana u gradu</p>

      {!isLoggedIn ? (
        <>
          {/* Kartica s ponudom */}
          <div className="kartica">
            <h2>Ponuda</h2>
            <p><strong>Dostupne članarine</strong></p>
            <ul className="lista">
              {offers.map((o) => (
                <li key={o.name}>
                  <span>{o.name}: </span>
                  <strong>{o.price}</strong>
                </li>
              ))}
            </ul>
          </div>

          {/* Kartica s radnim vremenom */}
          <div className="kartica">
            <h2>Gdje nas možete pronaći</h2>
            <p>Radno vrijeme i lokacija</p>
            <ul className="lista">
              <li>
                <span>Radno vrijeme: </span>
                <strong>pon-pet 6:00-22:00</strong>
              </li>
              <li>
                <span>Lokacija: </span>
                <strong>Ulica 1, 10000 Zagreb</strong>
              </li>
            </ul>
            <a className="gumb" href="tel:+385919483680">Nazovi nas</a>
          </div>

          {/* Forma za prijavu / registraciju */}
          <div className="kartica">
            <h2>{isRegisterMode ? 'Registracija' : 'Prijava'}</h2>
            <form onSubmit={isRegisterMode ? handleRegister : handleLogin} style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
              <input 
                type="email" 
                placeholder="E-mail" 
                value={email} 
                onChange={(e) => setEmail(e.target.value)} 
                required 
                style={{ padding: '10px' }}
              />
              <input 
                type="password" 
                placeholder="Lozinka" 
                value={password} 
                onChange={(e) => setPassword(e.target.value)} 
                required 
                style={{ padding: '10px' }}
              />

              {isRegisterMode && (
                <>
                  <label style={{ textAlign: 'left', fontSize: '0.9rem', color: '#fff' }}>Odaberite članarinu/program:</label>
                  <select 
                    value={selectedOffer} 
                    onChange={(e) => setSelectedOffer(e.target.value)}
                    style={{ padding: '10px', background: '#161616', color: '#fff', border: '2px solid #00ff66', borderRadius: '4px' }}
                  >
                    {offers.map(o => (
                      <option key={o.name} value={o.name}>
                        {o.name} ({o.price})
                      </option>
                    ))}
                  </select>
                </>
              )}

              <button type="submit" className="gumb" style={{ marginTop: '10px' }}>
                {isRegisterMode ? 'Potvrdi registraciju' : 'Prijavi se'}
              </button>
            </form>

            <p style={{ marginTop: '15px', cursor: 'pointer', textDecoration: 'underline', fontSize: '0.9rem', color: '#00ff66' }} onClick={() => setIsRegisterMode(!isRegisterMode)}>
              {isRegisterMode ? 'Već imate račun? Prijavite se' : 'Nemate račun? Registrirajte se'}
            </p>
          </div>
        </>
      ) : (
        /* AKO JE KORISNIK PRIJAVLJEN */
        <div style={{ width: '100%', maxWidth: '600px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
          
          <div className="kartica">
            <h2>Dobrodošli, {userEmail}!</h2>
            <p>Vaša aktivna članarina:</p>
            <div style={{ background: '#222', padding: '15px', border: '2px solid #00ff66', margin: '15px 0' }}>
              <strong>{activeOffer}</strong>
            </div>

            <button 
              onClick={handleLogout} 
              style={{ padding: '10px 20px', background: '#ff4444', color: '#fff', border: 'none', cursor: 'pointer', fontWeight: 'bold', borderRadius: '4px' }}
            >
              Odjavi se
            </button>
          </div>

          {/* Kartica za pretraživanje članova */}
          <div className="kartica">
            <h2>Pretraživanje članova</h2>
            <form onSubmit={handleSearch} style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
              <input 
                type="text" 
                placeholder="Upiši ime člana..." 
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                required
                style={{ padding: '10px', flex: 1, background: '#161616', border: '1px solid #00ff66', color: '#fff', borderRadius: '4px' }}
              />
              <button type="submit" className="gumb" style={{ width: 'auto', padding: '10px 20px' }}>Traži</button>
            </form>

            {searchResults.length > 0 && (
              <ul className="lista" style={{ textAlign: 'left' }}>
                {searchResults.map((m, index) => (
                  <li key={index} style={{ padding: '10px 0', borderBottom: '1px solid #333' }}>
                    <strong>{m.name || m.email}</strong> - Ponuda: <span style={{ color: '#00ff66' }}>{m.activeOffer || 'Nema'}</span>
                  </li>
                ))}
              </ul>
            )}
          </div>

        </div>
      )}
    </div>
  );
}

export default App;