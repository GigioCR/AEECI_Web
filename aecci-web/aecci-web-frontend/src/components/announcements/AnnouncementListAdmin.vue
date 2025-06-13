<template>
  <v-container>
    <v-card flat>
      <v-card-title class="d-flex align-center">
        <v-icon class="mr-2">mdi-bullhorn</v-icon>
        Gestión de Avisos
        <v-spacer />
        <v-btn color="primary" @click="openDialog()">Nuevo Aviso</v-btn>
      </v-card-title>
      <v-divider />

      <table class="custom-table">
        <thead>
          <tr>
            <th>Título</th>
            <th>Fecha</th>
            <th>Cuerpo</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="ann in announcements" :key="ann.id">
            <td>{{ ann.title }}</td>
            <td>{{ formatDate(ann.publicationDate) }}</td>
            <td>{{ ann.body }}</td>
            <td>
              <v-btn icon small color="blue" @click="editAnnouncement(ann)">
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
              <v-btn icon small color="red" @click="deleteAnnouncement(ann.id)">
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </td>
          </tr>
        </tbody>
      </table>
    </v-card>

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
              label="Fecha y Hora"
              type="datetime-local"
              :rules="dateRules"
              required
            />
            <v-text-field
              v-model="editedItem.imageUrl"
              label="URL Imagen (opcional)"
              :rules="imageUrlRules"
            />
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text color="gray" @click="closeDialog()">Cancelar</v-btn>
          <v-btn text color="green darken-1" :disabled="!isValid" @click="save()">
            Guardar
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

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

/*const headers = [
  { text: 'Título', value: 'title' },
  { text: 'Fecha', value: 'publicationDate' },
  { text: 'Acciones', value: 'actions', sortable: false }
]*/

const titleRules = [
  v => !!v || 'El título es obligatorio',
  v => v.length <= 200 || 'Máximo 200 caracteres'
]
const bodyRules = [ v => !!v || 'El cuerpo es obligatorio' ]
const dateRules = [ v => !!v || 'La fecha es obligatoria' ]
const imageUrlRules = [ v => !v || v.length <= 500 || 'Máximo 500 caracteres' ]

function formatDate(iso) {
  return new Date(iso).toLocaleString()
}

async function fetchAnnouncements() {
  try {
    const res = await api.get('/announcements')
    announcements.value = res.data
  } catch (e) {
    console.error('Error al cargar avisos:', e)
  }
}

function openDialog() {
  editedIndex.value = -1
  editedItem.value = { ...defaultItem }
  dialog.value = true
}

function editAnnouncement(item) {
  editedIndex.value = announcements.value.findIndex(a => a.id === item.id)
  editedItem.value = {
    ...item,
    publicationDate: item.publicationDate.slice(0, 16)
  }
  dialog.value = true
}

async function save() {
  if (!form.value.validate()) return
  try {
    if (editedIndex.value > -1) {
      await api.put(`/announcements/${editedItem.value.id}`, editedItem.value)
      announcements.value.splice(editedIndex.value, 1, { ...editedItem.value })
    } else {
      const res = await api.post('/announcements', editedItem.value)
      announcements.value.push(res.data)
    }
    closeDialog()
  } catch (e) {
    console.error('Error al guardar aviso:', e)
  }
}

async function deleteAnnouncement(id) {
  if (!confirm('¿Eliminar este aviso?')) return
  try {
    await api.delete(`/announcements/${id}`)
    announcements.value = announcements.value.filter(a => a.id !== id)
  } catch (e) {
    console.error('Error al eliminar aviso:', e)
  }
}

function closeDialog() {
  dialog.value = false
  form.value.resetValidation()
}

onMounted(fetchAnnouncements)
</script>

<style scoped>
.headline {
  font-weight: bold;
}
</style>
