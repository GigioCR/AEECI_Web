<template>
  <v-container fluid class="mt-8">
    <!-- Sección de anuncios -->
    <v-row>
      <v-col cols="12" class="text-center">
        <h1 class="mb-4">Avisos Recientes</h1>
      </v-col>
      <v-col
        v-for="announce in announcements"
        :key="announce.id"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card class="mb-4">
          <v-card-title>{{ announce.title }}</v-card-title>
          <v-card-subtitle class="grey--text">
            {{ formatDate(announce.publicationDate) }}
          </v-card-subtitle>
          <v-card-text>{{ announce.body }}</v-card-text>
          <v-card-actions v-if="announce.imageUrl">
            <v-img
              :src="announce.imageUrl"
              max-width="100"
              max-height="100"
            ></v-img>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Sección de catálogo de productos -->
    <v-divider class="my-8"></v-divider>
    <v-row>
      <v-col cols="12" class="text-center">
        <h1 class="mb-4">Catálogo de Productos</h1>
      </v-col>
      <v-col
        v-for="product in products"
        :key="product.id"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card class="mb-4">
          <v-img
            :src="product.imageUrl || defaultImage"
            height="150"
          ></v-img>
          <v-card-title>{{ product.name }}</v-card-title>
          <v-card-subtitle>
            Precio: ₡ {{ product.price.toFixed(2) }}
          </v-card-subtitle>
          <v-card-text>
            Cantidad Disponible: {{ product.quantity }}
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
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