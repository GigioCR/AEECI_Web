<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>AECCI Web</v-toolbar-title>
      <v-spacer />

      <v-btn
        icon
        class="d-sm-none"
        @click="drawer = !drawer"
        aria-label="Abrir menú"
      >
        <SvgIcon type="mdi" :path="mdiMenu" />
      </v-btn>

      <div class="d-none d-sm-flex">
        <v-btn text to="/">Inicio</v-btn>
        <v-btn v-if="auth.isAuthenticated" text to="/admin/products">Admin</v-btn>
        <v-btn v-if="!auth.isAuthenticated" text @click="router.push({ name: 'Login' })">Login</v-btn>
        <v-btn v-else text @click="logout()">Cerrar Sesión</v-btn>
      </div>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" temporary left class="d-sm-none">
      <v-list>
        <v-list-item to="/">
          <v-list-item-title>Inicio</v-list-item-title>
        </v-list-item>
        <v-list-item v-if="auth.isAuthenticated" to="/admin/products">
          <v-list-item-title>Admin</v-list-item-title>
        </v-list-item>
        <v-list-item v-if="!auth.isAuthenticated" @click="router.push({ name: 'Login' }); drawer = false">
          <v-list-item-title>Login</v-list-item-title>
        </v-list-item>
        <v-list-item v-else @click="logout(); drawer = false">
          <v-list-item-title>Cerrar Sesión</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from './store/auth'
import SvgIcon from '@jamescoyle/vue-icon';
import {mdiMenu } from '@mdi/js';
import { useRouter } from 'vue-router'

const drawer = ref(false)
const auth = useAuthStore()
const router = useRouter()

function logout() {
  auth.logout()
  router.push({ name: 'Home' })
}
</script>
