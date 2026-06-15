import { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import AddIcon from '@mui/icons-material/Add';
import RefreshIcon from '@mui/icons-material/Refresh';
import {
  Alert,
  Box,
  Button,
  Container,
  Stack,
  Typography,
} from '@mui/material';
import ConfirmDialog from './ConfirmDialog';
import OrderDialog from './OrderDialog';
import OrdersTable from './OrdersTable';
import { labels } from '../constants/labels';
import {
  clearError,
  clearMutationError,
  createOrder,
  deleteOrder,
  fetchOrders,
  updateOrder,
} from '../store/ordersSlice';

export default function OrdersPage() {
  const dispatch = useDispatch();
  const {
    list,
    loading,
    saving,
    deleting,
    error,
    mutationError,
  } = useSelector((state) => state.orders);

  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [orderToDelete, setOrderToDelete] = useState(null);

  const loadOrders = useCallback(() => {
    dispatch(fetchOrders());
  }, [dispatch]);

  useEffect(() => {
    loadOrders();
  }, [loadOrders]);

  const handleOpenCreate = () => {
    dispatch(clearMutationError());
    setSelectedOrder(null);
    setDialogOpen(true);
  };

  const handleOpenEdit = (order) => {
    dispatch(clearMutationError());
    setSelectedOrder(order);
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    if (!saving) {
      setDialogOpen(false);
      setSelectedOrder(null);
      dispatch(clearMutationError());
    }
  };

  const handleSubmit = async (dto) => {
    const action = selectedOrder
      ? await dispatch(updateOrder({ id: selectedOrder.id, dto }))
      : await dispatch(createOrder(dto));

    if (!action.meta.rejectedWithValue) {
      setDialogOpen(false);
      setSelectedOrder(null);
    }
  };

  const handleConfirmDelete = async () => {
    if (!orderToDelete) return;
    const action = await dispatch(deleteOrder(orderToDelete.id));
    if (!action.meta.rejectedWithValue) {
      setOrderToDelete(null);
    }
  };

  return (
    <>
      <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 3 }}>
        <Typography variant="h5" component="h2" fontWeight={600}>
          {labels.ordersTitle ?? 'Comenzi'}
        </Typography>
        <Stack direction="row" spacing={1}>
          <Typography variant="body2" sx={{ alignSelf: 'center', color: 'text.secondary' }}>
            {labels.totalOrders(list.length)}
          </Typography>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={loadOrders}
            disabled={loading}
          >
            {labels.refresh}
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleOpenCreate}
          >
            {labels.addOrder}
          </Button>
        </Stack>
      </Stack>

      <Container maxWidth="xl" disableGutters>
        <Stack spacing={3}>
          {error && (
            <Alert severity="error" onClose={() => dispatch(clearError())}>
              {error}
            </Alert>
          )}
          {mutationError && !dialogOpen && !orderToDelete && (
            <Alert severity="error" onClose={() => dispatch(clearMutationError())}>
              {mutationError}
            </Alert>
          )}

          <OrdersTable
            orders={list}
            loading={loading}
            onEdit={handleOpenEdit}
            onDelete={setOrderToDelete}
          />
        </Stack>
      </Container>

      <OrderDialog
        open={dialogOpen}
        order={selectedOrder}
        saving={saving}
        serverError={mutationError}
        onClose={handleCloseDialog}
        onSubmit={handleSubmit}
      />

      <ConfirmDialog
        open={Boolean(orderToDelete)}
        title={labels.confirmDeleteTitle}
        message={labels.confirmDeleteMessage}
        confirming={deleting}
        onCancel={() => !deleting && setOrderToDelete(null)}
        onConfirm={handleConfirmDelete}
      />
    </>
  );
}
