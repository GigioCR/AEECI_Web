<template>
  <v-app>
    <!-- Sidebar -->
    <v-navigation-drawer
      app
      color="#5B6984"
      rail
      expand-on-hover
      mini-variant-width="64"
      width="320"
    >
      <v-list dense>

        <!-- Logo -->
        <v-list-item>
          <v-list-item-icon>
            <v-img
              src="@/assets/logo.png"
              alt="Logo AECCI"
              max-width="32"
              contain
            />
          </v-list-item-icon>
        </v-list-item>

        <!-- Productos -->
        <v-list-item :to="{ name: 'Products' }" ripple>
          <template #prepend>
            <svg-icon type="mdi" :path="mdiArchiveEdit" />
          </template>
          <v-list-item-content>
            <v-list-item-title>Productos Disponibles</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <!-- Anuncios -->
        <v-list-item :to="{ name: 'Announcements' }" ripple>
          <template #prepend>
            <svg-icon type="mdi" :path="mdiBullhornOutline" />
          </template>
          <v-list-item-content>
            <v-list-item-title>Anuncios</v-list-item-title>
          </v-list-item-content>
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

import SvgIcon from '@jamescoyle/vue-icon'
import { mdiArchiveEdit, mdiBullhornOutline } from '@mdi/js'

const router = useRouter()
const auth = useAuthStore()

function logout() {
  auth.logout()
  router.push({ name: 'Home' })
}
</script>

<style scoped>
.v-list-item {
  justify-content: center;
}

.v-list-item-content {
  padding-left: 16px;
}
</style>
