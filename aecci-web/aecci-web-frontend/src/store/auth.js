import { defineStore } from 'pinia'
import api from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || ''
  }),
  getters: {
    isAuthenticated: state => !!state.token
  },
  actions: {
    async login(username, password) {
      try {
        const res = await api.post('/auth/login', { username, password })
        this.token = res.data.token
        localStorage.setItem('token', this.token)
        return true            // <<< retorna éxito
      } catch (error) {
        console.error('Login failed:', error)
        return false           // <<< retorna fallo
      }
    },
    logout() {
      this.token = ''
      localStorage.removeItem('token')
    }
  }
})
