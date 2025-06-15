<template>
  <v-container>
    <v-card flat class="pa-4">
      <v-card-title class="d-flex align-center mb-4">
        <svg-icon type="mdi" :path="mdiBullhorn"></svg-icon>
        <span class="text-h5 font-weight-bold">Gestión de Avisos</span>
        <v-spacer />
        <v-btn color="primary" @click="openDialog()" class="px-4 py-2 rounded-lg">Nuevo Aviso</v-btn>
      </v-card-title>
      <v-divider class="my-4" />

      <!-- Loading Indicator -->
      <v-row v-if="isLoadingAnnouncements" class="justify-center align-center" style="min-height: 400px;">
        <v-progress-circular indeterminate color="primary" size="64" />
      </v-row>

      <!-- Announcement Cards Layout -->
      <v-row v-else-if="announcements.length > 0">
        <v-col v-for="announcement in announcements" :key="announcement.id" cols="12" sm="6" md="4" lg="3">
          <v-card class="mx-auto my-4 rounded-lg elevation-3 d-flex flex-column h-100">
            <v-img src="https://placehold.co/300x200/E0E0E0/000000?text=No+Image" height="180px" cover class="rounded-t-lg" />
            <v-card-title class="text-h6 font-weight-bold">{{ announcement.title }}</v-card-title>
            <v-card-subtitle class="d-flex flex-column align-start">
              <span class="text-caption text-grey-darken-1">Publicado:</span>
              <span class="text-body-1 font-weight-medium">{{ formatDate(announcement.publishedDate) }}</span>
            </v-card-subtitle>
            <v-card-text class="flex-grow-1 text-body-2 text-grey-darken-2">{{ announcement.body }}</v-card-text>
            <v-card-actions class="justify-end pt-0 pb-4 px-4">
              <v-btn color="blue-darken-1" variant="tonal" size="small" class="mr-2" @click="editAnnouncement(announcement)">
                <svg-icon type="mdi" :path="mdiPencilOutline"></svg-icon>
                Editar
              </v-btn>
              <v-btn color="red-darken-1" variant="tonal" size="small" @click="deleteAnnouncement(announcement.id)">
                <svg-icon type="mdi" :path="mdiDelete"></svg-icon>
                Eliminar
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
        <v-col v-if="announcements.length===0" cols="12" class="text-center text-h6 text-grey-darken-1 py-10">No hay avisos disponibles.</v-col>
      </v-row>
    </v-card>

    <!-- Diálogo de creación/edición -->
    <v-dialog v-model="dialog" max-width="600px">
      <v-card class="rounded-lg">
        <v-card-title class="bg-primary text-white py-3 px-5 rounded-t-lg">
          <span class="headline">{{ editedIndex===-1?'Nuevo Aviso':'Editar Aviso' }}</span>
        </v-card-title>
        <v-card-text class="py-4 px-5">
          <v-form ref="form" v-model="isValid">
            <v-text-field v-model="editedItem.title" label="Título" :rules="titleRules" required variant="outlined" class="mb-4" />
            <v-textarea v-model="editedItem.body" label="Cuerpo del Aviso" :rules="bodyRules" auto-grow required variant="outlined" class="mb-4" />
          </v-form>
        </v-card-text>
        <v-card-actions class="justify-end bg-grey-lighten-4 py-3 px-5 rounded-b-lg">
          <v-btn text color="grey-darken-2" @click="closeDialog()" class="px-4 py-2 rounded-lg">Cancelar</v-btn>
          <v-btn text color="green-darken-2" :disabled="!isValid" @click="save()" class="px-4 py-2 rounded-lg">Guardar</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  <!---- borrar anuncio-->
    <v-dialog v-model="confirmDeleteDialog" max-width="450px">
            <v-card class="rounded-lg">
              <v-card-title class="bg-red-darken-2 text-white py-3 px-5 rounded-t-lg">
                Confirmar Eliminación
              </v-card-title>
              <v-card-text class="py-4 px-5 text-center text-body-1">
                ¿Estás seguro de que quieres eliminar este aviso?
                <br>
                <span class="font-weight-bold text-red-darken-2">{{ editedItem.title }}</span>
              </v-card-text>
              <v-card-actions class="justify-end bg-grey-lighten-4 py-3 px-5 rounded-b-lg">
                <v-btn text color="grey-darken-2" @click="cancelDelete()" class="px-4 py-2 rounded-lg">Cancelar</v-btn>
                <v-btn text color="red-darken-2" @click="confirmDelete()" class="px-4 py-2 rounded-lg">Eliminar</v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
  </v-container>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/services/api'
  import SvgIcon from '@jamescoyle/vue-icon'
  import { mdiBullhorn, mdiDelete, mdiPencilOutline } from '@mdi/js';

  const announcements = ref([])
  const isLoadingAnnouncements = ref(true)
  const dialog = ref(false)
  const isValid = ref(false)
  const confirmDeleteDialog = ref(false)
  const announcementToDeleteId = ref(null)
  const form = ref(null)
  const editedIndex = ref(-1)
  const defaultItem = { id:null, title:'', body:'', publishedDate:'' }
  const editedItem = ref({ ...defaultItem })

  const titleRules = [ v=>!!v||'El título es obligatorio', v=>v.length<=200||'Máximo 200 caracteres' ]
  const bodyRules  = [ v=>!!v||'El cuerpo es obligatorio' ]

  const formatDate = iso => new Date(iso).toLocaleDateString()

  async function fetchAnnouncements() {
    isLoadingAnnouncements.value = true
    try {
      const res = await api.get('/announcements')
      announcements.value = res.data
    } catch(e) { console.error('Error al cargar avisos:', e) }
    finally {
      isLoadingAnnouncements.value = false
    }
  }

  function openDialog() { editedIndex.value=-1; editedItem.value={...defaultItem}; dialog.value=true }
  function editAnnouncement(item) { editedIndex.value=announcements.value.findIndex(a=>a.id===item.id); editedItem.value={...item,publishedDate:item.publishedDate.slice(0,16)}; dialog.value=true }

  async function save() {
    if (!form.value.validate()) return
    try {
      if (editedIndex.value > -1) {
        await api.put(`/announcements/${editedItem.value.id}`, editedItem.value)
      } else {
        await api.post('/announcements', editedItem.value)
      }
      await fetchAnnouncements()
      closeDialog()
    } catch (e) {
      console.error('Error al guardar aviso:', e)
    }
  }


  function deleteAnnouncement(id) {
    announcementToDeleteId.value = id;
    const ann = announcements.value.find(a => a.id === id);
    if (ann) {
      editedItem.value.title = ann.title;
    }
    confirmDeleteDialog.value = true;
  }

  async function confirmDelete() {
    try {
      await api.delete(`/announcements/${announcementToDeleteId.value}`);
      announcements.value = announcements.value.filter(a => a.id !== announcementToDeleteId.value);
      console.log(`Announcement with ID ${announcementToDeleteId.value} deleted.`);
    } catch (e) {
      console.error('Error al eliminar aviso:', e);
    } finally {
      cancelDelete();
    }
  }


  function cancelDelete() {
    confirmDeleteDialog.value = false;
    announcementToDeleteId.value = null;
    editedItem.value.title = '';
  }

  function closeDialog(){ dialog.value=false; form.value.resetValidation() }

  onMounted(fetchAnnouncements)

</script>

<style scoped>
.headline{font-weight:bold}
</style>