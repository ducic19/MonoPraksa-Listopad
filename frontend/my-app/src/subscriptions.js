// The plan list needs a login, so guests see these numbers from the seed data.
export const FALLBACK_PLANS = [
  { subsId: 'seed-weekly', weekly: 7, duration: 1, price: 15 },
  { subsId: 'seed-monthly', weekly: 3, duration: 4, price: 45 },
  { subsId: 'seed-quarterly', weekly: 3, duration: 12, price: 120 },
  { subsId: 'seed-annual', weekly: 3, duration: 52, price: 400 },
];

// 45 -> "€45", 11.25 -> "€11.25"
export function formatPrice(price) {
  const value = Number(price);
  return `€${value % 1 === 0 ? value : value.toFixed(2)}`;
}

export function planName(plan) {
  const weeks = Number(plan.duration);
  if (weeks <= 1) return 'Weekly plan';
  if (weeks <= 4) return 'Monthly plan';
  if (weeks <= 13) return '3-month plan';
  return 'Annual plan';
}

// "3× a week · 4 weeks"
export function planInfo(plan) {
  const weeks = Number(plan.duration);
  return `${plan.weekly}× a week · ${weeks} ${weeks === 1 ? 'week' : 'weeks'}`;
}
