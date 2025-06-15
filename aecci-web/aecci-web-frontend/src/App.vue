<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>AECCI Web</v-toolbar-title>
      <v-spacer />
      <v-btn v-if="!auth.isAuthenticated" text to="/">Inicio</v-btn>
      
      <v-btn v-if="!auth.isAuthenticated" text @click="router.push({ name: 'Login' })">Login</v-btn>
      <v-btn v-else text @click="logout()">Cerrar Sesión</v-btn>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
import { useAuthStore } from './store/auth';
import { useRouter } from 'vue-router';
export default {
  name: 'App',
  setup() {
    const auth = useAuthStore();
    const router = useRouter();

    
    function logout() {
      auth.logout();
      router.push({ name: 'Home' });
    }

    
    return {
      auth,
      router,
      logout
    };
  },
};
</script>
