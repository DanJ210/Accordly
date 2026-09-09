import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import DashboardPage from '@/pages/DashboardPage.vue'
import AgreementsPage from '@/pages/AgreementsPage.vue'
import AgreementDetailPage from '@/pages/AgreementDetailPage.vue'
import LoginPage from '@/pages/LoginPage.vue'
import RegisterPage from '@/pages/RegisterPage.vue'
import GuestSignPage from '@/pages/GuestSignPage.vue'

const router = createRouter({ history: createWebHistory(), routes: [
  { path: '/', component: DashboardPage, meta: { auth: true } },
  { path: '/agreements', component: AgreementsPage, meta: { auth: true } },
  { path: '/agreements/:id', component: AgreementDetailPage, meta: { auth: true } },
  { path: '/sign/:token', component: GuestSignPage },
  { path: '/login', component: LoginPage },
  { path: '/register', component: RegisterPage },
] })
router.beforeEach((to) => to.meta.auth && !useAuthStore().token ? '/login' : true)
export default router
