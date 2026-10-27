import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import MemberSearch from './components/MemberSearch';
import SubscriptionCard from './components/SubscriptionCard';
import { apiFetch } from './api';
import { useAuth } from './auth-context';
import { FALLBACK_PLANS, formatPrice, planInfo, planName } from './subscriptions';

export default function App() {
  const { isLoggedIn } = useAuth();

  return (
    <div className="app">
      <Header />
      <main className="page">{isLoggedIn ? <Dashboard /> : <Home />}</main>
      <Footer />
    </div>
  );
}

function Home() {
  // The plans start as a short price list and grow into full cards on request.
  const [detailed, setDetailed] = useState(false);

  // Once the cards have grown, bring the whole list into view.
  useEffect(() => {
    if (detailed) document.getElementById('plans')?.scrollIntoView();
  }, [detailed]);

  return (
    <>
      <section className="hero container">
        <h1>
          Come as you are. <span className="blue">Leave stronger.</span>
        </h1>
        <p>Gym memberships in Zagreb. One price, no sign-up fee.</p>
        <div className="hero-actions">
          <Link to="/login?mode=register" className="btn btn-primary">
            Get started
          </Link>
          <button type="button" className="btn" onClick={() => setDetailed(!detailed)}>
            {detailed ? 'Hide details' : 'See plans'}
          </button>
        </div>
      </section>

      <section className={detailed ? 'container' : 'container plans-preview'} id="plans">
        <h2>Plans</h2>
        <div className="plan-grid">
          {FALLBACK_PLANS.map((plan) => (
            <SubscriptionCard key={plan.subsId} plan={plan} compact={!detailed} />
          ))}
        </div>
      </section>
    </>
  );
}

function Dashboard() {
  const { memberId, email } = useAuth();
  const [plans, setPlans] = useState([]);
  const [coaches, setCoaches] = useState([]);
  const [member, setMember] = useState(null);
  const [loading, setLoading] = useState(true);
  const [savingId, setSavingId] = useState('');
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');

  useEffect(() => {
    async function loadPlans() {
      try {
        setPlans(await apiFetch('subscription'));
      } catch (err) {
        setError(err.message);
      }
    }

    async function loadCoaches() {
      try {
        setCoaches(await apiFetch('trainer'));
      } catch {
        // The coach list is a nice extra, so we just leave it empty.
      }
    }

    async function loadMember() {
      try {
        if (memberId) setMember(await apiFetch(`member/${memberId}/details`));
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadPlans();
    loadCoaches();
    loadMember();
  }, [memberId]);

  async function choosePlan(plan) {
    if (savingId) return;

    setSavingId(plan.subsId);
    setError('');
    setNotice('');

    try {
      setMember(await apiFetch(`member/me/subscription/${plan.subsId}`, { method: 'PUT' }));
      setNotice(`${planName(plan)} activated.`);
    } catch (err) {
      setError(err.message);
    } finally {
      setSavingId('');
    }
  }

  const current = member?.subscription;
  const activePlan = current
    ? plans.find((plan) => plan.weekly === current.weekly && plan.duration === current.duration)
    : null;
  const shownPlans = plans.length ? plans : FALLBACK_PLANS;
  const canChoose = plans.length > 0;
  const fullName = member?.name || email;
  const firstName = fullName.split(' ')[0].split('@')[0];

  return (
    <>
      <section className="container dash-head">
        <h1>
          Hey, <span className="blue">{firstName}</span>.
        </h1>
        <Link to="/search" className="btn">
          Find a member
        </Link>
      </section>

      {error && (
        <div className="container">
          <p className="alert alert-error" role="alert">
            {error}
          </p>
        </div>
      )}

      <section className="container">
        {loading ? (
          <p className="muted">Loading…</p>
        ) : current ? (
          <div className="card membership">
            <div>
              <h2>{planName(current)}</h2>
              <p className="membership-price">{formatPrice(current.price)}</p>
              <p>{planInfo(current)}</p>
            </div>
            <span className="tag">Active</span>
          </div>
        ) : (
          <div className="card membership">
            <div>
              <h2>No plan yet</h2>
              <p>Pick one below — you can switch anytime.</p>
            </div>
            <a className="btn btn-primary" href="#plans">
              Choose a plan
            </a>
          </div>
        )}
      </section>

      <section className="container" id="plans">
        <h2>Plans</h2>

        {notice && (
          <p className="alert alert-success" role="status">
            {notice}
          </p>
        )}

        <div className="plan-grid">
          {shownPlans.map((plan) => (
            <SubscriptionCard
              key={plan.subsId}
              plan={plan}
              active={activePlan?.subsId === plan.subsId}
              pending={savingId === plan.subsId}
              onSelect={canChoose ? choosePlan : undefined}
            />
          ))}
        </div>
      </section>

      <section className="container">
        <MemberSearch plans={shownPlans} />
      </section>

      {coaches.length > 0 && (
        <section className="container">
          <h2>Coaches</h2>
          <ul className="coach-list">
            {coaches.map((coach) => (
              <li key={coach.trainerId}>
                <p className="coach-name">{coach.name}</p>
                <a className="coach-email" href={`mailto:${coach.email}`}>
                  {coach.email}
                </a>
              </li>
            ))}
          </ul>
        </section>
      )}
    </>
  );
}
