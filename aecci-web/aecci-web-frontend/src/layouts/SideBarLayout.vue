<template>
  <v-app>
    <!-- Sidebar -->
    <v-navigation-drawer app permanent clipped>
      <v-list dense>
        <v-list-item :to="{ name: 'Products' }">
          <v-list-item-title>Productos Disponibles</v-list-item-title>
        </v-list-item>
        <v-list-item :to="{ name: 'Announcements' }">
          <v-list-item-title>Anuncios</v-list-item-title>
        </v-list-item>
        <v-list-item :to="{ name: 'Info' }">
          <v-list-item-title>Contacto y Horarios</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <!-- Top Bar -->
    <v-app-bar app clipped-left>
      <v-toolbar-title>AECCI Web – Panel Admin</v-toolbar-title>
      <v-spacer />
      <v-btn text @click="logout()">Cerrar Sesión</v-btn>
    </v-app-bar>

    <!-- Main Content -->
    <v-main>
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/store/auth'

const router = useRouter()
const auth = useAuthStore()

function logout() {
  auth.logout()
  router.push({ name: 'Home' })
}
</script>
