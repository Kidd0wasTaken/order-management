import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import * as ordersApi from '../api/ordersApi';
import { getErrorMessage } from '../utils/formatters';
import { labels } from '../constants/labels';

export const fetchOrders = createAsyncThunk(
  'orders/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      return await ordersApi.fetchOrders();
    } catch (error) {
      return rejectWithValue(getErrorMessage(error, labels.errorLoading));
    }
  },
);

export const createOrder = createAsyncThunk(
  'orders/create',
  async (dto, { rejectWithValue }) => {
    try {
      return await ordersApi.createOrder(dto);
    } catch (error) {
      return rejectWithValue(getErrorMessage(error, labels.errorSaving));
    }
  },
);

export const updateOrder = createAsyncThunk(
  'orders/update',
  async ({ id, dto }, { rejectWithValue }) => {
    try {
      return await ordersApi.updateOrder(id, dto);
    } catch (error) {
      return rejectWithValue(getErrorMessage(error, labels.errorSaving));
    }
  },
);

export const deleteOrder = createAsyncThunk(
  'orders/delete',
  async (id, { rejectWithValue }) => {
    try {
      await ordersApi.deleteOrder(id);
      return id;
    } catch (error) {
      return rejectWithValue(getErrorMessage(error, labels.errorDeleting));
    }
  },
);

const initialState = {
  list: [],
  loading: false,
  saving: false,
  deleting: false,
  error: null,
  mutationError: null,
};

const ordersSlice = createSlice({
  name: 'orders',
  initialState,
  reducers: {
    clearMutationError(state) {
      state.mutationError = null;
    },
    clearError(state) {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrders.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload;
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(createOrder.pending, (state) => {
        state.saving = true;
        state.mutationError = null;
      })
      .addCase(createOrder.fulfilled, (state, action) => {
        state.saving = false;
        state.list = [action.payload, ...state.list];
      })
      .addCase(createOrder.rejected, (state, action) => {
        state.saving = false;
        state.mutationError = action.payload;
      })
      .addCase(updateOrder.pending, (state) => {
        state.saving = true;
        state.mutationError = null;
      })
      .addCase(updateOrder.fulfilled, (state, action) => {
        state.saving = false;
        state.list = state.list.map((order) =>
          order.id === action.payload.id ? action.payload : order,
        );
      })
      .addCase(updateOrder.rejected, (state, action) => {
        state.saving = false;
        state.mutationError = action.payload;
      })
      .addCase(deleteOrder.pending, (state) => {
        state.deleting = true;
        state.mutationError = null;
      })
      .addCase(deleteOrder.fulfilled, (state, action) => {
        state.deleting = false;
        state.list = state.list.filter((order) => order.id !== action.payload);
      })
      .addCase(deleteOrder.rejected, (state, action) => {
        state.deleting = false;
        state.mutationError = action.payload;
      });
  },
});

export const { clearMutationError, clearError } = ordersSlice.actions;
export default ordersSlice.reducer;
