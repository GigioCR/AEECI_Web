<template>
  <v-container fluid class="mt-8">
    <!-- Store Status Display -->
    <v-row justify="center" class="mb-6">
      <v-col cols="12" class="text-center">
        <div v-if="isLoadingStatus" class="flex-center" style="min-height: 48px;">
          <v-progress-circular indeterminate color="primary" size="32" />
        </div>
        <v-alert
          v-else
          :type="isStoreOpen ? 'success' : 'error'"
          :icon="isStoreOpen ? 'mdi-check-circle' : 'mdi-alert-circle'"
          prominent
          density="compact"
          class="rounded-lg shadow-md max-w-xl mx-auto"
        >
          <span class="font-weight-bold text-uppercase">
            {{ isStoreOpen ? 'Abierto' : 'Cerrado' }}
          </span>
          <span v-if="isStoreOpen" class="ml-2">¡Estamos Abiertos!</span>
          <span v-else class="ml-2">La tienda está Cerrada temporalmente.</span>
        </v-alert>
      </v-col>
    </v-row>

    <!-- Sección de anuncios -->
    <v-row>
      <v-col cols="12" class="text-center">
        <h1 class="mb-4">Avisos Recientes</h1>
      </v-col>
      <v-col cols="12">
        <!-- Cargando para anuncios -->
        <v-row
          v-if="isLoadingAnnouncements"
          class="justify-center align-center"
          style="min-height: 400px;"
        >
          <v-progress-circular indeterminate color="primary" size="64" />
        </v-row>

        <!-- Announcements Carousel -->
        <v-carousel
          v-else-if="announcements.length > 0"
          cycle
          height="450"
          hide-delimiter-background
          show-arrows="hover"
          class="rounded-lg elevation-4"
        >
          <v-carousel-item v-for="announce in announcements" :key="announce.id">
            <v-card class="d-flex flex-column h-100 rounded-lg">
              <v-img
                :src="announce.imageUrl || 'https://placehold.co/800x450/444444/FFFFFF?text=Sin+Imagen+para+Aviso'"
                height="250px"
                cover
                class="rounded-t-lg"
              >
                <v-row class="fill-height ma-0" align="end" justify="center">
                  <v-card-title
                    class="text-white text-h5 text-shadow-lg"
                    style="background: rgba(0,0,0,0.5); width: 100%; padding: 16px;"
                  >
                    {{ announce.title }}
                  </v-card-title>
                </v-row>
              </v-img>
              <v-card-subtitle class="mt-2 text-grey-darken-1 text-right pr-4">
                Publicado: {{ formatDate(announce.publishedDate) }}
              </v-card-subtitle>
              <v-card-text class="flex-grow-1 text-body-1 text-grey-darken-2 px-4 py-2">
                {{ announce.body }}
              </v-card-text>
            </v-card>
          </v-carousel-item>
        </v-carousel>
        <!-- Mensaje de que no hay anuncios -->
        <v-col v-else cols="12" class="text-center text-h6 text-grey-darken-1 py-10">
          No hay avisos recientes disponibles.
        </v-col>
      </v-col>
    </v-row>

    <!-- Sección de catálogo de productos -->
    <v-divider class="my-8"></v-divider>
    <v-row>
      <v-col cols="12" class="text-center">
        <h1 class="mb-4">Catálogo de Productos</h1>
      </v-col>
      <v-col cols="12">
        <!-- Cargando productos -->
        <v-row
          v-if="isLoadingProducts"
          class="justify-center align-center"
          style="min-height: 400px;"
        >
          <v-progress-circular indeterminate color="primary" size="64" />
        </v-row>

        <!-- Productos -->
        <v-row v-else-if="products.length > 0">
          <v-col v-for="product in products" :key="product.id" cols="12" sm="6" md="4" lg="3">
            <v-card class="mx-auto my-4 rounded-lg elevation-3 d-flex flex-column h-100">
              <v-img
                :src="product.imageUrl || 'https://placehold.co/300x200/E0E0E0/000000?text=No+Image'"
                height="180px"
                cover
                class="rounded-t-lg"
              />
              <v-card-title class="text-h6 font-weight-bold">
                {{ product.name }}
              </v-card-title>
              <v-card-subtitle class="d-flex flex-column align-start">
                <span class="text-caption text-grey-darken-1">Precio:</span>
                <span class="text-body-1 font-weight-medium">₡{{ product.price.toFixed(2) }}</span>
                <div class="mt-2">
                  <v-chip
                    :color="product.quantity > 0 ? 'green-darken-1' : 'red-darken-1'"
                    label
                    size="small"
                    class="text-uppercase"
                  >
                    {{ product.quantity > 0 ? 'Disponible' : 'Agotado' }}
                  </v-chip>
                  <v-chip
                    v-if="product.quantity > 0"
                    label
                    size="small"
                    class="ml-2 bg-blue-grey-lighten-4 text-blue-grey-darken-3"
                  >
                    Cant: {{ product.quantity }}
                  </v-chip>
                </div>
              </v-card-subtitle>
              <v-card-text class="flex-grow-1 text-body-2 text-grey-darken-2">
                {{ product.description }}
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
        <!-- Mensaje de que no hay productos -->
        <v-col v-else cols="12" class="text-center text-h6 text-grey-darken-1 py-10">
          No hay productos disponibles.
        </v-col>
      </v-col>
    </v-row>

    <!-- Footer -->
    <v-app>
      <v-footer app color="primary" class="d-flex flex-column text-white py-6">
        <v-container class="w-100">
          <h2 class="text-h5 font-weight-bold mb-4 text-center">Contacto y Horarios</h2>
          <v-row justify="center" class="text-center">
            <!-- Teléfono -->
            <v-col cols="12" sm="6" md="3" class="d-flex flex-column align-center mb-4 mb-md-0">
              <v-icon size="28" class="mb-2">mdi-phone</v-icon>
              <span class="font-weight-bold">Teléfono</span>
              <span>8888-8888</span>
            </v-col>
            <!-- Email -->
            <v-col cols="12" sm="6" md="3" class="d-flex flex-column align-center mb-4 mb-md-0">
              <v-icon size="28" class="mb-2">mdi-email</v-icon>
              <span class="font-weight-bold">Email</span>
              <span>aecci@ucr.ac.cr</span>
            </v-col>
            <!-- Horario -->
            <v-col cols="12" sm="6" md="3" class="d-flex flex-column align-center mb-4 mb-md-0">
              <v-icon size="28" class="mb-2">mdi-clock</v-icon>
              <span class="font-weight-bold">Horario</span>
              <span>Lun-Vie 08:00-21:00</span>
            </v-col>
            <!-- Ubicación -->
            <v-col cols="12" sm="6" md="3" class="d-flex flex-column align-center">
              <v-icon size="28" class="mb-2">mdi-map-marker</v-icon>
              <span class="font-weight-bold">Ubicación</span>
              <span>2º piso, ECCI, UCR</span>
            </v-col>
          </v-row>
          <v-divider class="my-6 border-opacity-50" />
          <div class="text-center text-caption">&copy; {{ new Date().getFullYear() }} AECCI. Todos los derechos reservados.</div>
        </v-container>
      </v-footer>
    </v-app>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const announcements = ref([])
const products = ref([])
const isLoadingAnnouncements = ref(true)
const isLoadingProducts = ref(true)
const isLoadingStatus = ref(true)
const isStoreOpen = ref(false)

const formatDate = iso => {
  const d = new Date(iso)
  return d.toLocaleDateString()
}

const fetchAnnouncements = async () => {
  isLoadingAnnouncements.value = true
  try {
    const res = await api.get('/Announcements')
    announcements.value = res.data
  } catch (e) {
    console.error('No se pudieron cargar los avisos', e)
  } finally {
    isLoadingAnnouncements.value = false
  }
}

const fetchProducts = async () => {
  isLoadingProducts.value = true
  try {
    const res = await api.get('/products')
    products.value = res.data
  } catch (e) {
    console.error('Error al cargar productos:', e)
  } finally {
    isLoadingProducts.value = false
  }
}

const fetchStoreStatus = async () => {
  isLoadingStatus.value = true
  try {
    const res = await api.get('/StoreStatus')
    if (res.data && typeof res.data.isOpen === 'boolean') {
      isStoreOpen.value = res.data.isOpen
    }
  } catch (e) {
    console.error('Error al cargar el estado de la tienda:', e)
  } finally {
    isLoadingStatus.value = false
  }
}

onMounted(() => {
  fetchStoreStatus()
  fetchAnnouncements()
  fetchProducts()
})
</script>

<style scoped>
h1 { font-weight: bold }
</style>