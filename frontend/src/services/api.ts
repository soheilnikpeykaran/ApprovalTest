import type { AuthResponse, FormSchema, RequestItem } from '../types';

const API_BASE = import.meta.env.VITE_API_URL ?? 'https://localhost:7001/api';

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('access_token');
  const headers = new Headers(options.headers);
  headers.set('Content-Type', 'application/json');
  if (token) headers.set('Authorization', `Bearer ${token}`);
  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  const body = await response.json().catch(() => null);
  if (!response.ok) throw new Error(body?.message ?? `Request failed (${response.status})`);
  return body as T;
}

export const api = {
  login: (email: string, password: string) => request<AuthResponse>('/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) }),
  register: (data: { email: string; password: string; firstName: string; lastName: string }) => request<AuthResponse>('/auth/register', { method: 'POST', body: JSON.stringify(data) }),
  getSchema: () => request<FormSchema>('/form-schema'),
  getRequests: () => request<RequestItem[]>('/requests'),
  createRequest: (data: Record<string, unknown>) => request<RequestItem>('/requests', { method: 'POST', body: JSON.stringify(data) }),
  decide: (id: string, action: 'approve' | 'reject') => request<RequestItem>(`/requests/${id}/decision`, { method: 'POST', body: JSON.stringify({ action }) })
};
