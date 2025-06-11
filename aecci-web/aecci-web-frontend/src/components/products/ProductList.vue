<template>
  <v-container>
    <v-row justify="space-between" align="center" class="mb-4">
      <v-col cols="auto"><h2>Productos</h2></v-col>
      <v-col cols="auto">
        <v-btn color="primary" @click="openDialog()">Nuevo Producto</v-btn>
      </v-col>
    </v-row>

    <v-data-table
      :headers="headers"
      :items="products"
      :items-per-page="5"
      class="elevation-1"
    >
      <template v-slot:item.actions="{ item }">
        <v-icon small class="mr-2" color="blue" @click="editProduct(item)">
          mdi-pencil
        </v-icon>
        <v-icon small color="red" @click="deleteProduct(item.id)">
          mdi-delete
        </v-icon>
      </template>
    </v-data-table>

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
              label="Precio (₡)"
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
              label="URL de Imagen"
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
  name: 'ProductList',
  setup() {
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

    const headers = [
      { text: 'Nombre', value: 'name' },
      { text: 'Precio (₡)', value: 'price' },
      { text: 'Cantidad', value: 'quantity' },
      { text: 'Disponible', value: 'isAvailable' },
      { text: 'Acciones', value: 'actions', sortable: false }
    ]

    const nameRules = [
      v => !!v || 'El nombre es obligatorio',
      v => v.length <= 100 || 'Máximo 100 caracteres'
    ]
    const descriptionRules = [
      v => !!v || 'La descripción es obligatoria',
      v => v.length <= 500 || 'Máximo 500 caracteres'
    ]
    const priceRules = [
      v => v !== null && v !== undefined || 'El precio es obligatorio',
      v => v >= 0 || 'El precio debe ser ≥ 0'
    ]
    const quantityRules = [
      v => v !== null && v !== undefined || 'La cantidad es obligatoria',
      v => Number.isInteger(v) && v >= 0 || 'La cantidad debe ser entero ≥ 0'
    ]
    const imageUrlRules = [
      v => !v || v.length <= 200 || 'Máximo 200 caracteres'
    ]

    const fetchProducts = async () => {
      try {
        const res = await api.get('/products')
        products.value = res.data
      } catch (err) {
        console.error('Error al cargar productos:', err)
      }
    }

    const openDialog = () => {
      editedIndex.value = -1
      editedItem.value = { ...defaultItem }
      dialog.value = true
    }

    const editProduct = item => {
      editedIndex.value = products.value.findIndex(p => p.id === item.id)
      editedItem.value = { ...item }
      dialog.value = true
    }

    const save = async () => {
      if (!form.value.validate()) return

      try {
        if (editedIndex.value > -1) {
          // Actualizar
          await api.put(`/products/${editedItem.value.id}`, editedItem.value)
          products.value.splice(editedIndex.value, 1, { ...editedItem.value })
        } else {
          // Crear
          const res = await api.post('/products', editedItem.value)
          products.value.push(res.data)
        }
        closeDialog()
      } catch (err) {
        console.error('Error al guardar:', err)
      }
    }

    const deleteProduct = async id => {
      if (!confirm('¿Eliminar este producto?')) return
      try {
        await api.delete(`/products/${id}`)
        products.value = products.value.filter(p => p.id !== id)
      } catch (err) {
        console.error('Error al eliminar:', err)
      }
    }

    const closeDialog = () => {
      dialog.value = false
      form.value.resetValidation()
    }

    onMounted(fetchProducts)

    return {
      products,
      headers,
      dialog,
      isValid,
      form,
      editedItem,
      editedIndex,
      nameRules,
      descriptionRules,
      priceRules,
      quantityRules,
      imageUrlRules,
      openDialog,
      editProduct,
      save,
      deleteProduct,
      closeDialog
    }
  }
}
</script>

<style scoped>
.headline {
  font-weight: bold;
}
</style>
