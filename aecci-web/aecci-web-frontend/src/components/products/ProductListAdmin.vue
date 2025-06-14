<template>
  <v-container>
          <v-card flat class="pa-4">
            <v-card-title class="d-flex align-center mb-4">
              <v-icon class="mr-2">mdi-package-variant</v-icon>
              <span class="text-h5 font-weight-bold">Inventario de Productos</span>
              <v-spacer />
              <v-btn color="primary" @click="openDialog()" class="px-4 py-2 rounded-lg">
                Nuevo Producto
              </v-btn>
            </v-card-title>
            <v-divider class="my-4" />

            <!-- Product Cards Layout -->
            <v-row>
              <v-col
                v-for="product in products"
                :key="product.id"
                cols="12"
                sm="6"
                md="4"
                lg="3"
              >
                <v-card class="mx-auto my-4 rounded-lg elevation-3 d-flex flex-column h-100">
                  <v-img
                    :src="product.imageUrl || 'https://placehold.co/300x200/E0E0E0/000000?text=No+Image'"
                    height="180px"
                    cover
                    class="rounded-t-lg"
                  >
                  </v-img>
                  <v-card-title class="text-h6 font-weight-bold">
                    {{ product.name }}
                  </v-card-title>
                  <v-card-subtitle class="d-flex flex-column align-start">
                    <span class="text-caption text-grey-darken-1">Precio:</span>
                    <span class="text-body-1 font-weight-medium">₡{{ product.price.toFixed(2) }}</span>
                    <div class="mt-2">
                      <v-chip
                        :color="product.isAvailable ? 'green-darken-1' : 'red-darken-1'"
                        label
                        size="small"
                        class="text-uppercase"
                      >
                        {{ product.isAvailable ? 'Disponible' : 'Agotado' }}
                      </v-chip>
                      <v-chip
                        v-if="product.quantity > 0"
                        label
                        size="small"
                        class="ml-2 bg-blue-grey-lighten-4 text-blue-grey-darken-3"
                      >
                        Cant: {{ product.quantity }}
                      </v-chip>
                    </div>
                  </v-card-subtitle>
                  <v-card-text class="flex-grow-1 text-body-2 text-grey-darken-2">
                    {{ product.description }}
                  </v-card-text>
                  <v-card-actions class="justify-end pt-0 pb-4 px-4">
                    <v-btn
                      color="blue-darken-1"
                      variant="tonal"
                      size="small"
                      class="mr-2"
                      @click="editProduct(product)"
                    >
                      <v-icon start>mdi-pencil</v-icon>
                      Editar
                    </v-btn>
                    <v-btn
                      color="red-darken-1"
                      variant="tonal"
                      size="small"
                      @click="deleteProduct(product.id)"
                    >
                      <v-icon start>mdi-delete</v-icon>
                      Eliminar
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-col>
              <v-col v-if="products.length === 0" cols="12" class="text-center text-h6 text-grey-darken-1 py-10">
                No hay productos disponibles.
              </v-col>
            </v-row>

          </v-card>

          <!-- Diálogo de creación/edición -->
          <v-dialog v-model="dialog" max-width="600px">
            <v-card class="rounded-lg">
              <v-card-title class="bg-primary text-white py-3 px-5 rounded-t-lg">
                <span class="headline">
                  {{ editedIndex === -1 ? 'Nuevo Producto' : 'Editar Producto' }}
                </span>
              </v-card-title>
              <v-card-text class="py-4 px-5">
                <v-form ref="form" v-model="isValid">
                  <v-text-field
                    v-model="editedItem.name"
                    label="Nombre"
                    :rules="nameRules"
                    required
                    variant="outlined"
                    class="mb-4"
                  />
                  <v-textarea
                    v-model="editedItem.description"
                    label="Descripción"
                    :rules="descriptionRules"
                    auto-grow
                    required
                    variant="outlined"
                    class="mb-4"
                  />
                  <v-text-field
                    v-model.number="editedItem.price"
                    label="Precio"
                    type="number"
                    :rules="priceRules"
                    required
                    variant="outlined"
                    class="mb-4"
                  />
                  <v-text-field
                    v-model.number="editedItem.quantity"
                    label="Cantidad"
                    type="number"
                    :rules="quantityRules"
                    required
                    variant="outlined"
                    class="mb-4"
                  />
                  <v-switch
                    v-model="editedItem.isAvailable"
                    label="Disponible"
                    color="primary"
                    inset
                    class="mb-4"
                  />
                  <v-text-field
                    v-model="editedItem.imageUrl"
                    label="URL Imagen"
                    :rules="imageUrlRules"
                    variant="outlined"
                    class="mb-4"
                  />
                </v-form>
              </v-card-text>
              <v-card-actions class="justify-end bg-grey-lighten-4 py-3 px-5 rounded-b-lg">
                <v-btn text color="grey-darken-2" @click="closeDialog()" class="px-4 py-2 rounded-lg">Cancelar</v-btn>
                <v-btn text color="green-darken-2" :disabled="!isValid" @click="save()" class="px-4 py-2 rounded-lg">
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
