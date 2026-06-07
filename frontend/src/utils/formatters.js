export function formatCurrency(value) {
  return new Intl.NumberFormat('ro-RO', {
    style: 'currency',
    currency: 'RON',
  }).format(value ?? 0);
}

export function formatDate(value) {
  if (!value) return '—';
  return new Intl.DateTimeFormat('ro-RO', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

export function getErrorMessage(error, fallback) {
  return (
    error?.response?.data?.message
    ?? error?.response?.data?.title
    ?? error?.message
    ?? fallback
  );
}
