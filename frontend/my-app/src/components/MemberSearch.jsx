import { useState } from 'react';
import { apiFetch } from '../api';
import { planName } from '../subscriptions';

export default function MemberSearch({ plans = [] }) {
  const [name, setName] = useState('');
  const [results, setResults] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function handleSubmit(event) {
    event.preventDefault();
    if (!name.trim()) return;

    setLoading(true);
    setError('');

    try {
      const data = await apiFetch(`member/search?name=${encodeURIComponent(name)}`);
      setResults(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  function findPlan(subsId) {
    return plans.find((plan) => plan.subsId === subsId);
  }

  return (
    <div className="card">
      <h2>Members</h2>

      <form className="search-form" onSubmit={handleSubmit}>
        <input
          className="input"
          type="search"
          placeholder="Search by name"
          aria-label="Member name"
          value={name}
          onChange={(event) => setName(event.target.value)}
        />
        <button type="submit" className="btn btn-primary" disabled={loading || !name.trim()}>
          {loading ? 'Searching…' : 'Search'}
        </button>
      </form>

      {error && (
        <p className="alert alert-error" role="alert">
          {error}
        </p>
      )}

      {results && results.length === 0 && <p className="muted">No members found.</p>}

      {results && results.length > 0 && (
        <ul className="member-list">
          {results.map((member) => (
            <li key={member.memId}>
              <div>
                <p className="member-name">{member.name}</p>
                <p className="member-email">{member.email}</p>
              </div>
              <span className="member-plan">
                {findPlan(member.subsId) ? planName(findPlan(member.subsId)) : 'No plan'}
              </span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
