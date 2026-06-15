import axios from 'axios';

const client = axios.create({
  baseURL: '/api/d100',
  headers: { 'Content-Type': 'application/json' },
});

export async function fetchCompany() {
  const { data } = await client.get('/company');
  return data;
}

export async function saveCompany(dto) {
  const { data } = await client.put('/company', dto);
  return data;
}

export async function previewDeclaration(period) {
  const { data } = await client.post('/preview', period);
  return data;
}

export async function generateDeclaration(period) {
  const { data } = await client.post('/generate', period);
  return data;
}

export async function generatePdf(id) {
  const { data } = await client.post(`/${id}/pdf`);
  return data;
}

export function getXmlDownloadUrl(id) {
  return `/api/d100/${id}/xml`;
}

export function getPdfDownloadUrl(id) {
  return `/api/d100/${id}/pdf`;
}
