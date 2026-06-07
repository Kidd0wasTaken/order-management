import { useEffect, useState } from 'react';
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from '@mui/material';
import { labels } from '../constants/labels';
import { ORDER_STATUSES, STATUS_LABELS, STATUS_OPTIONS } from '../constants/orderStatus';

const emptyForm = {
  customerName: '',
  product: '',
  quantity: 1,
  price: 0,
  status: ORDER_STATUSES.Pending,
  notes: '',
};

function buildFormState(order) {
  if (!order) return emptyForm;
  return {
    customerName: order.customerName ?? '',
    product: order.product ?? '',
    quantity: order.quantity ?? 1,
    price: order.price ?? 0,
    status: order.status ?? ORDER_STATUSES.Pending,
    notes: order.notes ?? '',
  };
}

function validate(form) {
  const errors = {};
  if (!form.customerName.trim()) errors.customerName = labels.requiredField;
  if (!form.product.trim()) errors.product = labels.requiredField;
  if (!form.quantity || Number(form.quantity) < 1) errors.quantity = labels.minQuantity;
  if (form.price === '' || Number(form.price) < 0) errors.price = labels.minPrice;
  return errors;
}

export default function OrderDialog({
  open,
  order,
  saving,
  serverError,
  onClose,
  onSubmit,
}) {
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    if (open) {
      setForm(buildFormState(order));
      setErrors({});
    }
  }, [open, order]);

  const isEdit = Boolean(order?.id);

  const handleChange = (field) => (event) => {
    const value = event.target.value;
    setForm((prev) => ({ ...prev, [field]: value }));
    setErrors((prev) => ({ ...prev, [field]: undefined }));
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    const validationErrors = validate(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    onSubmit({
      customerName: form.customerName.trim(),
      product: form.product.trim(),
      quantity: Number(form.quantity),
      price: Number(form.price),
      status: form.status,
      notes: form.notes.trim() || null,
    });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>{isEdit ? labels.editOrder : labels.newOrder}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label={labels.customer}
              value={form.customerName}
              onChange={handleChange('customerName')}
              error={Boolean(errors.customerName)}
              helperText={errors.customerName}
              required
              fullWidth
              autoFocus
            />
            <TextField
              label={labels.product}
              value={form.product}
              onChange={handleChange('product')}
              error={Boolean(errors.product)}
              helperText={errors.product}
              required
              fullWidth
            />
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <TextField
                label={labels.quantity}
                type="number"
                value={form.quantity}
                onChange={handleChange('quantity')}
                error={Boolean(errors.quantity)}
                helperText={errors.quantity}
                inputProps={{ min: 1 }}
                required
                fullWidth
              />
              <TextField
                label={labels.price}
                type="number"
                value={form.price}
                onChange={handleChange('price')}
                error={Boolean(errors.price)}
                helperText={errors.price}
                inputProps={{ min: 0, step: '0.01' }}
                required
                fullWidth
              />
            </Stack>
            <FormControl fullWidth required error={Boolean(errors.status)}>
              <InputLabel id="status-label">{labels.status}</InputLabel>
              <Select
                labelId="status-label"
                label={labels.status}
                value={form.status}
                onChange={handleChange('status')}
              >
                {STATUS_OPTIONS.map((status) => (
                  <MenuItem key={status} value={status}>
                    {STATUS_LABELS[status]}
                  </MenuItem>
                ))}
              </Select>
              {errors.status && <FormHelperText>{errors.status}</FormHelperText>}
            </FormControl>
            <TextField
              label={labels.notes}
              value={form.notes}
              onChange={handleChange('notes')}
              multiline
              minRows={3}
              fullWidth
            />
            {serverError && (
              <FormHelperText error sx={{ mx: 0 }}>
                {serverError}
              </FormHelperText>
            )}
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2 }}>
          <Button onClick={onClose} disabled={saving}>
            {labels.cancel}
          </Button>
          <Button type="submit" variant="contained" disabled={saving}>
            {labels.save}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
