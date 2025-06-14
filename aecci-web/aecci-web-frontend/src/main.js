import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import SvgIcon from '@jamescoyle/vue-icon'

import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives  from 'vuetify/directives'
import 'vuetify/styles'                   // estilos base de Vuetify

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'light',
  },
})

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(SvgIcon)
app.use(vuetify)

app.mount('#app')