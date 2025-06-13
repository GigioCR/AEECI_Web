<template>
  <v-container>
    <v-card flat>
      <v-card-title class="d-flex align-center">
        <v-icon class="mr-2">mdi-package-variant</v-icon>
        Inventario de Productos
        <v-spacer />
        <v-btn color="primary" @click="openDialog()">Nuevo Producto</v-btn>
      </v-card-title>
      <v-divider />

      <table class="custom-table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Precio (₡)</th>
            <th>Cantidad</th>
            <th>Disponible</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in products" :key="product.id">
            <td>{{ product.name }}</td>
            <td>₡{{ product.price.toFixed(2) }}</td>
            <td>{{ product.quantity }}</td>
            <td>{{ product.isAvailable ? 'Sí' : 'No' }}</td>
            <td>
              <v-btn icon small color="blue" @click="editProduct(product)">
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
              <v-btn icon small color="red" @click="deleteProduct(product.id)">
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
            {{ editedIndex === -1 ? 'Nuevo Producto' : 'Editar Producto' }}
          </span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="isValid">
            <v-text-field
              v-model="editedItem.name"
              label="Nombre"
              :rules="nameRules"
              required
            />
            <v-textarea
              v-model="editedItem.description"
              label="Descripción"
              :rules="descriptionRules"
              auto-grow
              required
            />
            <v-text-field
              v-model.number="editedItem.price"
              label="Precio"
              type="number"
              :rules="priceRules"
              required
            />
            <v-text-field
              v-model.number="editedItem.quantity"
              label="Cantidad"
              type="number"
              :rules="quantityRules"
              required
            />
            <v-switch
              v-model="editedItem.isAvailable"
              label="Disponible"
            />
            <v-text-field
              v-model="editedItem.imageUrl"
              label="URL Imagen"
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

const products = ref([])
const dialog = ref(false)
const isValid = ref(false)
const form = ref(null)
const editedIndex = ref(-1)
const defaultItem = {
  id: null,
  name: '',
  description: '',
  price: 0.0,
  quantity: 0,
  isAvailable: true,
  imageUrl: ''
}
const editedItem = ref({ ...defaultItem })

/*const headers = [
  { text: 'Nombre', value: 'name' },
  { text: 'Precio (₡)', value: 'price' },
  { text: 'Cantidad', value: 'quantity' },
  { text: 'Disponible', value: 'isAvailable' },
  { text: 'Acciones', value: 'actions', sortable: false }
]*/

const nameRules = [
  v => !!v || 'El nombre es obligatorio',
  v => v.length <= 100 || 'Máximo 100 caracteres'
]
const descriptionRules = [
  v => !!v || 'La descripción es obligatoria',
  v => v.length <= 500 || 'Máximo 500 caracteres'
]
const priceRules = [
  v => v != null || 'El precio es obligatorio',
  v => v >= 0 || 'El precio debe ser ≥ 0'
]
const quantityRules = [
  v => Number.isInteger(v) || 'La cantidad debe ser un entero',
  v => v >= 0 || 'La cantidad debe ser ≥ 0'
]
const imageUrlRules = [
  v => !v || v.length <= 200 || 'Máximo 200 caracteres'
]

async function fetchProducts() {
  try {
    const res = await api.get('/products')
    products.value = res.data
  } catch (e) {
    console.error('Error al cargar productos:', e)
  }
}

function openDialog() {
  editedIndex.value = -1
  editedItem.value = { ...defaultItem }
  dialog.value = true
}

function editProduct(item) {
  editedIndex.value = products.value.findIndex(p => p.id === item.id)
  editedItem.value = { ...item }
  dialog.value = true
}

async function save() {
  if (!form.value.validate()) return
  try {
    if (editedIndex.value > -1) {
      await api.put(`/products/${editedItem.value.id}`, editedItem.value)
      products.value.splice(editedIndex.value, 1, { ...editedItem.value })
    } else {
      const res = await api.post('/products', editedItem.value)
      products.value.push(res.data)
    }
    closeDialog()
  } catch (e) {
    console.error('Error al guardar producto:', e)
  }
}

async function deleteProduct(id) {
  if (!confirm('¿Eliminar este producto?')) return
  try {
    await api.delete(`/products/${id}`)
    products.value = products.value.filter(p => p.id !== id)
  } catch (e) {
    console.error('Error al eliminar producto:', e)
  }
}

function closeDialog() {
  dialog.value = false
  form.value.resetValidation()
}

onMounted(fetchProducts)
</script>

<style scoped>
.headline {
  font-weight: bold;
}
</style>
