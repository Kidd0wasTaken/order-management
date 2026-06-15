import { configureStore } from '@reduxjs/toolkit';
import ordersReducer from './ordersSlice';
import d100Reducer from './d100Slice';

export const store = configureStore({
  reducer: {
    orders: ordersReducer,
    d100: d100Reducer,
  },
});
