<template>
  <v-container>
    <v-row justify="space-between" align="center" class="mb-4">
      <v-col cols="auto"><h2>Avisos</h2></v-col>
      <v-col cols="auto">
        <v-btn color="primary" @click="openDialog()">Nuevo Aviso</v-btn>
      </v-col>
    </v-row>

    <v-data-table
      :headers="headers"
      :items="announcements"
      :items-per-page="5"
      class="elevation-1"
    >
      <template v-slot:item.publicationDate="{ item }">
        {{ formatDate(item.publicationDate) }}
      </template>
      <template v-slot:item.actions="{ item }">
        <v-icon small class="mr-2" color="blue" @click="editAnnouncement(item)">
          mdi-pencil
        </v-icon>
        <v-icon small color="red" @click="deleteAnnouncement(item.id)">
          mdi-delete
        </v-icon>
      </template>
    </v-data-table>

    <!-- Diálogo de creación/edición -->
    <v-dialog v-model="dialog" max-width="600px">
      <v-card>
        <v-card-title>
          <span class="headline">
            {{ editedIndex === -1 ? 'Nuevo Aviso' : 'Editar Aviso' }}
          </span>
        </v-card-title>

        <v-card-text>
          <v-form ref="form" v-model="isValid">
            <v-text-field
              v-model="editedItem.title"
              label="Título"
              :rules="titleRules"
              required
            />
            <v-textarea
              v-model="editedItem.body"
              label="Cuerpo del Aviso"
              :rules="bodyRules"
              auto-grow
              required
            />
            <v-text-field
              v-model="editedItem.publicationDate"
              label="Fecha de Publicación"
              type="datetime-local"
              :rules="dateRules"
              required
            />
            <v-text-field
              v-model="editedItem.imageUrl"
              label="URL de Imagen (opcional)"
              :rules="imageUrlRules"
            />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn text color="gray" @click="closeDialog()">Cancelar</v-btn>
          <v-btn text color="green darken-1" @click="save()" :disabled="!isValid">
            Guardar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

export default {
  name: 'AnnouncementList',
  setup() {
    const announcements = ref([])
    const dialog = ref(false)
    const isValid = ref(false)
    const form = ref(null)
    const editedIndex = ref(-1)
    const defaultItem = {
      id: null,
      title: '',
      body: '',
      publicationDate: '',
      imageUrl: ''
    }
    const editedItem = ref({ ...defaultItem })

    const headers = [
      { text: 'Título', value: 'title' },
      { text: 'Fecha', value: 'publicationDate' },
      { text: 'Acciones', value: 'actions', sortable: false }
    ]

    const titleRules = [
      v => !!v || 'El título es obligatorio',
      v => v.length <= 200 || 'Máximo 200 caracteres'
    ]
    const bodyRules = [
      v => !!v || 'El cuerpo es obligatorio'
    ]
    const dateRules = [
      v => !!v || 'La fecha es obligatoria'
    ]
    const imageUrlRules = [
      v => !v || v.length <= 500 || 'Máximo 500 caracteres'
    ]

    const fetchAnnouncements = async () => {
      try {
        const res = await api.get('/announcements')
        announcements.value = res.data
      } catch (err) {
        console.error('Error al cargar avisos:', err)
      }
    }

    const openDialog = () => {
      editedIndex.value = -1
      editedItem.value = { ...defaultItem }
      dialog.value = true
    }

    const editAnnouncement = item => {
      editedIndex.value = announcements.value.findIndex(a => a.id === item.id)
      // convert Date to ISO string for datetime-local input
      editedItem.value = {
        ...item,
        publicationDate: item.publicationDate.slice(0, 16)
      }
      dialog.value = true
    }

    const save = async () => {
      if (!form.value.validate()) return

      try {
        // convert back to full ISO if needed
        if (editedIndex.value > -1) {
          await api.put(`/announcements/${editedItem.value.id}`, editedItem.value)
          announcements.value.splice(editedIndex.value, 1, { ...editedItem.value })
        } else {
          const res = await api.post('/announcements', editedItem.value)
          announcements.value.push(res.data)
        }
        closeDialog()
      } catch (err) {
        console.error('Error al guardar aviso:', err)
      }
    }

    const deleteAnnouncement = async id => {
      if (!confirm('¿Eliminar este aviso?')) return
      try {
        await api.delete(`/announcements/${id}`)
        announcements.value = announcements.value.filter(a => a.id !== id)
      } catch (err) {
        console.error('Error al eliminar aviso:', err)
      }
    }

    const closeDialog = () => {
      dialog.value = false
      form.value.resetValidation()
    }

    const formatDate = iso => {
      const d = new Date(iso)
      return d.toLocaleString()
    }

    onMounted(fetchAnnouncements)

    return {
      announcements,
      headers,
      dialog,
      isValid,
      form,
      editedItem,
      editedIndex,
      titleRules,
      bodyRules,
      dateRules,
      imageUrlRules,
      openDialog,
      editAnnouncement,
      save,
      deleteAnnouncement,
      closeDialog,
      formatDate
    }
  }
}
</script>

<style scoped>
.headline {
  font-weight: bold;
}
</style>
