import axios from 'axios';

const client = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export async function fetchOrders() {
  const { data } = await client.get('/orders');
  return data;
}

export async function createOrder(dto) {
  const { data } = await client.post('/orders', dto);
  return data;
}

export async function updateOrder(id, dto) {
  const { data } = await client.put(`/orders/${id}`, dto);
  return data;
}

export async function deleteOrder(id) {
  await client.delete(`/orders/${id}`);
  return id;
}
