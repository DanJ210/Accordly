import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '@/composables/useApi'
export interface Agreement { id: string; title: string; status: string; ownerId: string; createdAt: string; updatedAt: string; expiresAt?: string }
export const useAgreementsStore = defineStore('agreements', () => {
  const agreements = ref<Agreement[]>([])
  const currentAgreement = ref<Agreement | null>(null)
  async function fetchAll() { agreements.value = (await api.get('/agreements')).data }
  async function fetchById(id: string) { currentAgreement.value = (await api.get(`/agreements/${id}`)).data }
  async function create(payload: { title: string; expiresAt?: string }) { const result = await api.post('/agreements', payload); return result.data as Agreement }
  async function update(id: string, payload: Partial<Agreement>) { await api.patch(`/agreements/${id}`, payload) }
  return { agreements, currentAgreement, fetchAll, fetchById, create, update }
})
