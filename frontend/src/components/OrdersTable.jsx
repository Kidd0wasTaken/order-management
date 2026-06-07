import {
  Box,
  Chip,
  CircularProgress,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { labels } from '../constants/labels';
import { getStatusLabel, STATUS_COLORS } from '../constants/orderStatus';
import { formatCurrency, formatDate } from '../utils/formatters';

export default function OrdersTable({
  orders,
  loading,
  onEdit,
  onDelete,
}) {
  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress aria-label={labels.loading} />
      </Box>
    );
  }

  if (!orders.length) {
    return (
      <Paper sx={{ p: 6, textAlign: 'center' }}>
        <Typography color="text.secondary">{labels.empty}</Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
      <Table size="medium" aria-label={labels.appTitle}>
        <TableHead>
          <TableRow>
            <TableCell>{labels.id}</TableCell>
            <TableCell>{labels.customer}</TableCell>
            <TableCell>{labels.product}</TableCell>
            <TableCell align="right">{labels.quantity}</TableCell>
            <TableCell align="right">{labels.price}</TableCell>
            <TableCell>{labels.status}</TableCell>
            <TableCell>{labels.notes}</TableCell>
            <TableCell>{labels.createdAt}</TableCell>
            <TableCell align="center">{labels.actions}</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {orders.map((order) => (
            <TableRow key={order.id} hover>
              <TableCell>{order.id}</TableCell>
              <TableCell sx={{ fontWeight: 500 }}>{order.customerName}</TableCell>
              <TableCell>{order.product}</TableCell>
              <TableCell align="right">{order.quantity}</TableCell>
              <TableCell align="right">{formatCurrency(order.price)}</TableCell>
              <TableCell>
                <Chip
                  label={getStatusLabel(order.status)}
                  color={STATUS_COLORS[order.status] ?? 'default'}
                  size="small"
                  variant="outlined"
                />
              </TableCell>
              <TableCell sx={{ maxWidth: 200 }}>
                <Typography variant="body2" noWrap title={order.notes ?? ''}>
                  {order.notes || '—'}
                </Typography>
              </TableCell>
              <TableCell>{formatDate(order.createdAt)}</TableCell>
              <TableCell align="center">
                <Tooltip title={labels.edit}>
                  <IconButton
                    size="small"
                    color="primary"
                    onClick={() => onEdit(order)}
                    aria-label={`${labels.edit} ${order.id}`}
                  >
                    <EditOutlinedIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
                <Tooltip title={labels.delete}>
                  <IconButton
                    size="small"
                    color="error"
                    onClick={() => onDelete(order)}
                    aria-label={`${labels.delete} ${order.id}`}
                  >
                    <DeleteOutlineIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
