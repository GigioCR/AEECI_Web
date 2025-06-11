<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>AECCI Web</v-toolbar-title>
      <v-spacer />
      <v-btn text to="/">Inicio</v-btn>
      <v-btn text to="/login">Login</v-btn>
      <v-btn text to="/admin">Admin</v-btn>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: 'App'
}
</script>
