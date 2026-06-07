import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@mui/material';
import { labels } from '../constants/labels';

export default function ConfirmDialog({
  open,
  title,
  message,
  onCancel,
  onConfirm,
  confirming = false,
}) {
  return (
    <Dialog open={open} onClose={onCancel} maxWidth="xs" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{message}</DialogContentText>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={onCancel} disabled={confirming}>
          {labels.cancel}
        </Button>
        <Button
          onClick={onConfirm}
          color="error"
          variant="contained"
          disabled={confirming}
        >
          {labels.delete}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
