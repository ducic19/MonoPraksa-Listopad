import { formatPrice, planInfo, planName } from '../subscriptions';

export default function SubscriptionCard({
  plan,
  active = false,
  pending = false,
  compact = false,
  onSelect,
}) {
  let label = 'Choose plan';
  if (active) label = 'Current plan';
  else if (pending) label = 'Saving…';

  return (
    <article className={`plan${active ? ' plan-active' : ''}${compact ? ' plan-compact' : ''}`}>
      <h3>{planName(plan)}</h3>
      <p className="plan-price">
        {formatPrice(plan.price)}
        <span className="plan-period">/ total</span>
      </p>

      {!compact && <p className="plan-info">{planInfo(plan)}</p>}

      {onSelect && (
        <button
          type="button"
          className={active ? 'btn' : 'btn btn-primary'}
          disabled={active || pending}
          onClick={() => onSelect(plan)}
        >
          {label}
        </button>
      )}
    </article>
  );
}
