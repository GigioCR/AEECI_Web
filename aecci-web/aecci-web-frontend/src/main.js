import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import vuetify from './plugins/vuetify'
import { createPinia } from 'pinia'
import SvgIcon from '@jamescoyle/vue-icon'


const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(SvgIcon)
app.use(vuetify)

app.mount('#app')