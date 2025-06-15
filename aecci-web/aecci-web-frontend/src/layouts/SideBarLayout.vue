<template>
  <v-app>
    <v-app-bar v-if="display.smAndDown" app color="#5B6984" class="text-white">
      <v-btn icon @click="drawer = !drawer">
        <svg-icon type="mdi" :path="mdiMenu" class="text-white" />
      </v-btn>
      <v-toolbar-title class="font-weight-bold"></v-toolbar-title>
      </v-app-bar>

    <v-navigation-drawer
      v-model="drawer"
      :permanent="display.mdAndUp"       :temporary="display.smAndDown"     :expand-on-hover="display.mdAndUp" :rail="display.mdAndUp"            mini-variant-width="64"
      width="320"
      color="#5B6984"
      class="text-white"
      app
    >
      <v-list dense nav>
        <v-list-item class="d-flex align-center">
          <template #prepend>
            <v-avatar size="32" class="rounded-lg">
              <v-img
                :src="aecci_logo"
                alt="Logo AECCI"
                contain
              />
            </v-avatar>
          </template>
          <v-list-item-title class="ml-2 font-weight-bold text-h6">AECCI ADMIN</v-list-item-title>
        </v-list-item>

        <v-divider class="my-2 border-opacity-50" />

        <v-list-item :to="{ name: 'Products' }" ripple link color="blue-lighten-2">
          <template #prepend>
            <svg-icon type="mdi" :path="mdiArchiveEdit" />
          </template>
          <v-list-item-content>
            <v-list-item-title>Productos Disponibles</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'Announcements' }" ripple link color="blue-lighten-2">
          <template #prepend>
            <svg-icon type="mdi" :path="mdiBullhornOutline" />
          </template>
          <v-list-item-content>
            <v-list-item-title>Anuncios</v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item @click="openStatusDialog" ripple link color="blue-lighten-2">
          <template #prepend>
            <svg-icon type="mdi" :path="mdiStore" />
          </template>
          <v-list-item-content>
            <v-list-item-title>Estado de la Tienda</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>

    <v-dialog v-model="confirmDialog" max-width="400">
      <v-card>
        <v-card-title class="headline">Cambiar estado de la tienda</v-card-title>
        <v-card-text>
          ¿Está seguro de que desea <strong>{{ isStoreOpen ? 'cerrar' : 'abrir' }}</strong> la tienda?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text color="grey-darken-2" @click="confirmDialog = false">Cancelar</v-btn>
          <v-btn text color="primary" @click="toggleStoreStatus">Sí</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="resultDialog" max-width="400">
      <v-card>
        <v-card-title class="headline">Estado Actualizado</v-card-title>
        <v-card-text>
          La tienda ahora está <strong>{{ isStoreOpen ? 'abierta' : 'cerrada' }}</strong>.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text color="primary" @click="resultDialog = false">OK</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-app>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useDisplay } from 'vuetify';
import SvgIcon from '@jamescoyle/vue-icon';
import { mdiArchiveEdit, mdiBullhornOutline, mdiStore, mdiMenu } from '@mdi/js';
import aecci_logo from '@/assets/aecci_logo.png';
import api from '@/services/api';

// Reactive state
const drawer = ref(null);
const isStoreOpen = ref(false);
const isLoadingStatus = ref(true);
const confirmDialog = ref(false);
const resultDialog = ref(false);

// Vuetify display composable
const display = useDisplay();

// Fetch current store status
const fetchStoreStatus = async () => {
  isLoadingStatus.value = true;
  try {
    const res = await api.get('/StoreStatus');
    if (res.data && typeof res.data.isOpen === 'boolean') {
      isStoreOpen.value = res.data.isOpen;
    }
  } catch (e) {
    console.error('Error al cargar estado de tienda:', e);
  } finally {
    isLoadingStatus.value = false;
  }
};

// Open confirmation dialog
const openStatusDialog = () => {
  fetchStoreStatus();
  confirmDialog.value = true;
};

// Toggle status via backend
const toggleStoreStatus = async () => {
  confirmDialog.value = false;
  try {
    await api.put('/StoreStatus', { id: 1, isOpen: !isStoreOpen.value });
    isStoreOpen.value = !isStoreOpen.value;
    resultDialog.value = true;
  } catch (e) {
    console.error('Error al cambiar estado de tienda:', e);
  }
};

// On mount, load status
onMounted(() => {
  fetchStoreStatus();
  drawer.value = display.mdAndUp.value; // Drawer is open by default on larger screens
});
</script>

<style scoped>
.v-list-item {
  justify-content: center;
}

.v-list-item-content {
  padding-left: 16px;
}
</style>