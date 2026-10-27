import { useEffect, useState } from 'react';
import Header from './components/Header';
import Footer from './components/Footer';
import MemberSearch from './components/MemberSearch';
import { apiFetch } from './api';
import { useAuth } from './auth-context';

export default function Search() {
  const { isLoggedIn } = useAuth();
  const [plans, setPlans] = useState([]);

  useEffect(() => {
    if (!isLoggedIn) return;

    apiFetch('subscription')
      .then(setPlans)
      .catch(() => {
        // Search still works, it just can't show plan names.
      });
  }, [isLoggedIn]);

  return (
    <div className="app">
      <Header />
      <main className="page">
        <section className="container">
          {isLoggedIn ? (
            <MemberSearch plans={plans} />
          ) : (
            <div className="card">
              <h1>Members</h1>
              <p className="muted">Log in to search the member directory.</p>
            </div>
          )}
        </section>
      </main>
      <Footer />
    </div>
  );
}
