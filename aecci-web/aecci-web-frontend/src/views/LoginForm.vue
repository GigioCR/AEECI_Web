<template>
  <v-container class="fill-height d-flex flex-column justify-center align-center">
    <h2 class="mb-6 text-3xl font-bold text-gray-800 text-center">Inicio de Sesión</h2>
    <v-card max-width="800" min-width="400" class="pa-4">
      <v-form ref="form" v-model="valid">
        <v-text-field
          v-model="user"
          label="Usuario"
          :rules="[v => !!v || 'Requerido']"
          required
        />
        <v-text-field
          v-model="pass"
          label="Contraseña"
          type="password"
          :rules="[v => !!v || 'Requerido']"
          required
        />
        <v-alert v-if="error" type="error" dense>{{ error }}</v-alert>
        <v-card-actions class="justify-end">
          <v-btn color="primary" :disabled="!valid" @click="onSubmit">
            Ingresar
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/store/auth'

const user  = ref('')
const pass  = ref('')
const valid = ref(false)
const error = ref('')
const form  = ref(null)

const router = useRouter()
const auth = useAuthStore()

async function onSubmit() {
  if (!form.value.validate()) return
  console.log('Enviando credenciales', user.value, pass.value)
  const ok = await auth.login(user.value, pass.value)
  console.log('auth.login devolvió:', ok)
  if (ok) {
    router.push({ name: 'Products' })
  } else {
    error.value = 'Credenciales inválidas'
  }
}
</script>
