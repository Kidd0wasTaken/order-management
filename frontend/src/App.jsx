import { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import AddIcon from '@mui/icons-material/Add';
import RefreshIcon from '@mui/icons-material/Refresh';
import {
  Alert,
  AppBar,
  Box,
  Button,
  Container,
  Stack,
  Toolbar,
  Typography,
} from '@mui/material';
import ConfirmDialog from './components/ConfirmDialog';
import OrderDialog from './components/OrderDialog';
import OrdersTable from './components/OrdersTable';
import { labels } from './constants/labels';
import {
  clearError,
  clearMutationError,
  createOrder,
  deleteOrder,
  fetchOrders,
  updateOrder,
} from './store/ordersSlice';

export default function App() {
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
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar position="static" elevation={0} sx={{ borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
        <Toolbar>
          <Typography variant="h6" component="h1" sx={{ flexGrow: 1, fontWeight: 700 }}>
            {labels.appTitle}
          </Typography>
          <Typography variant="body2" sx={{ mr: 2, opacity: 0.85 }}>
            {labels.totalOrders(list.length)}
          </Typography>
          <Button
            color="inherit"
            startIcon={<RefreshIcon />}
            onClick={loadOrders}
            disabled={loading}
            sx={{ mr: 1 }}
          >
            {labels.refresh}
          </Button>
          <Button
            variant="contained"
            color="secondary"
            startIcon={<AddIcon />}
            onClick={handleOpenCreate}
          >
            {labels.addOrder}
          </Button>
        </Toolbar>
      </AppBar>

      <Container maxWidth="xl" sx={{ py: 4 }}>
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
    </Box>
  );
}
