export const ORDER_STATUSES = {
  Pending: 'Pending',
  Processing: 'Processing',
  Completed: 'Completed',
  Cancelled: 'Cancelled',
};

export const STATUS_OPTIONS = Object.values(ORDER_STATUSES);

export const STATUS_LABELS = {
  [ORDER_STATUSES.Pending]: 'În așteptare',
  [ORDER_STATUSES.Processing]: 'În procesare',
  [ORDER_STATUSES.Completed]: 'Finalizată',
  [ORDER_STATUSES.Cancelled]: 'Anulată',
};

export const STATUS_COLORS = {
  [ORDER_STATUSES.Pending]: 'warning',
  [ORDER_STATUSES.Processing]: 'info',
  [ORDER_STATUSES.Completed]: 'success',
  [ORDER_STATUSES.Cancelled]: 'error',
};

export function getStatusLabel(status) {
  return STATUS_LABELS[status] ?? status;
}
