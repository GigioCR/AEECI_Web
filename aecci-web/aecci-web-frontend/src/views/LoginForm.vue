<template>
  <v-card class="mx-auto pa-4" max-width="400">
    <v-form ref="form" v-model="valid">
      <v-text-field v-model="user" label="Usuario" :rules="[v=>!!v||'Requerido']" required/>
      <v-text-field v-model="pass" label="Contraseña" type="password" :rules="[v=>!!v||'Requerido']" required/>
      <v-alert v-if="error" type="error" dense>{{ error }}</v-alert>
      <v-card-actions>
        <v-spacer/>
        <v-btn color="primary" :disabled="!valid" @click="onSubmit">Ingresar</v-btn>
      </v-card-actions>
    </v-form>
  </v-card>
</template>

<script>
import { ref } from 'vue'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'

export default {
  setup() {
    const user  = ref('')
    const pass  = ref('')
    const valid = ref(false)
    const error = ref('')
    const form  = ref(null)
    const auth  = useAuthStore()
    const router= useRouter()

    const onSubmit = async () => {
      if (!form.value.validate()) return
      console.log('Enviando credenciales', user.value, pass.value)

      const ok = await auth.login(user.value, pass.value)
      console.log('auth.login devolvió:', ok)
      if (ok) router.push({ name: 'Admin' })
      else   error.value = 'Credenciales inválidas'
    }

    return { user, pass, valid, error, form, onSubmit }
  }
}
</script>
 