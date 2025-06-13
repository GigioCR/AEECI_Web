<template>
  <v-container class="fill-height d-flex justify-center align-center">
    <v-sheet elevation="2" rounded class="pa-12 text-center">
      <h1>¡Bienvenidos a AECCI Web!</h1>
      <p>Consulta nuestros productos y avisos, o inicia sesión para gestionar el inventario.</p>
      <v-btn color="primary" @click="$router.push({ name: 'Login' })">
        Iniciar Sesión
      </v-btn>
    </v-sheet>
  </v-container>
</template>


<script>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

export default {
  name: 'HomeView',
  setup() {
    const announcements = ref([])
    const products      = ref([])
    const defaultImage  = 'https://via.placeholder.com/150'

    const formatDate = iso => {
      const d = new Date(iso)
      return d.toLocaleString()
    }

    const fetchAnnouncements = async () => {
      try {
        const res = await api.get('/Announcements')
        announcements.value = res.data
      } catch (e) {
        console.error('No se pudieron cargar los avisos', e)
      }
    }

    const fetchProducts = async () => {
      try {
        const res = await api.get('/Products')
        // filtrar solo disponibles si lo deseas:
        products.value = res.data.filter(p => p.isAvailable)
      } catch (e) {
        console.error('No se pudo cargar los productos', e)
      }
    }

    onMounted(() => {
      fetchAnnouncements()
      fetchProducts()
    })

    return {
      announcements,
      products,
      defaultImage,
      formatDate
    }
  }
}
</script>

<style scoped>
h1 {
  font-weight: bold;
}
</style>
