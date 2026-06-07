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

function extractValidationErrors(errors) {
  if (!errors || typeof errors !== 'object') {
    return null;
  }

  const messages = Object.values(errors)
    .flat()
    .filter((message) => typeof message === 'string' && message.length > 0);

  return messages.length > 0 ? messages.join(' ') : null;
}

export function getErrorMessage(error, fallback) {
  const data = error?.response?.data;

  if (typeof data === 'string' && data.length > 0) {
    return data;
  }

  if (data && typeof data === 'object') {
    const validationMessage = extractValidationErrors(data.errors);
    if (validationMessage) {
      return validationMessage;
    }

    if (typeof data.message === 'string' && data.message.length > 0) {
      return data.message;
    }

    if (typeof data.title === 'string' && data.title.length > 0) {
      return data.title;
    }
  }

  return error?.message ?? fallback;
}
