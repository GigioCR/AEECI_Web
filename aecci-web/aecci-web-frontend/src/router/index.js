import { createRouter, createWebHistory } from 'vue-router'
import HomeView        from '@/views/HomeView.vue'
import HomeViewAdmin from '@/views/HomeViewAdmin.vue'
import LoginView       from '@/views/LoginForm.vue'
import ProductListAdmin     from '@/components/products/ProductListAdmin.vue'
import AnnouncementListAdmin from '@/components/announcements/AnnouncementListAdmin.vue'
import InfoView        from '@/views/InfoView.vue'
import { useAuthStore } from '@/store/auth'
import SidebarLayout   from '@/layouts/SideBarLayout.vue'



const routes = [
  {path: '/admin/home', name: 'Home', component: HomeViewAdmin},
  { path: '/',      name: 'Home',  component: HomeView },
  {
    path: '/login',
    name: 'Login',
    component: LoginView
  },
  {
    path: '/admin',
    component: SidebarLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'products',
        name: 'Products',
        component: ProductListAdmin
      },
      {
        path: 'announcements',
        name: 'Announcements',
        component: AnnouncementListAdmin
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
