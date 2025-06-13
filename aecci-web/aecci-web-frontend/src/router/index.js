import { createRouter, createWebHistory } from 'vue-router'
import HomeView        from '@/views/HomeView.vue'
import LoginView       from '@/views/LoginForm.vue'
import ProductList     from '@/components/products/ProductList.vue'
import AnnouncementList from '@/components/announcements/AnnouncementList.vue'
import InfoView        from '@/views/InfoView.vue'
import { useAuthStore } from '@/store/auth'
import SidebarLayout   from '@/layouts/SideBarLayout.vue'



const routes = [
  {path: '/', name: 'Home', component: HomeView},
  {
    path: '/login',
    name: 'Login',
    component: LoginView
  },
  {
    path: '/',
    component: SidebarLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'products',
        name: 'Products',
        component: ProductList
      },
      {
        path: 'announcements',
        name: 'Announcements',
        component: AnnouncementList
      },
      {
        path: 'info',
        name: 'Info',
        component: InfoView
      },
      {
        path: '',
        redirect: { name: 'Products' }
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
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
