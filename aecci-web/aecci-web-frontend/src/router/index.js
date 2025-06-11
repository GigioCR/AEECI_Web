import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginForm.vue'
//import AdminView from '@/views/AdminView.vue'
import { useAuthStore } from '@/store/auth'

const routes = [
  { path: '/',      name: 'Home',  component: HomeView },
  { path: '/login', name: 'Login', component: LoginView },
  //{ path: '/admin', name: 'Admin', component: AdminView, meta: { requiresAuth: true } },
  { path: '/:pathMatch(.*)*', redirect: '/' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return next({ name: 'Login' })
  }
  next()
})

export default router
