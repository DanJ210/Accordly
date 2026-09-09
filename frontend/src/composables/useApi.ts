import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

export const api = axios.create({ baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api/v1' })
api.interceptors.request.use((config) => {
  const token = useAuthStore().token
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})
api.interceptors.response.use(undefined, (error) => {
  if (error.response?.status === 401) window.location.href = '/login'
  return Promise.reject(error)
})
export function useApi() { return api }
