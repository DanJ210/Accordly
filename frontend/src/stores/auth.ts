import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '@/composables/useApi'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('accordly_token'))
  const user = ref<{ displayName: string; email: string } | null>(null)
  async function login(email: string, password: string) {
    const response = await api.post('/auth/login', { email, password })
    token.value = response.data.accessToken
    if (token.value) localStorage.setItem('accordly_token', token.value)
  }
  async function register(email: string, displayName: string, password: string) {
    await api.post('/auth/register', { email, displayName, password })
  }
  function logout() { token.value = null; user.value = null; localStorage.removeItem('accordly_token') }
  return { token, user, login, register, logout }
})
