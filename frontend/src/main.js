import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { Dialog, Notify, Quasar } from 'quasar'

import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(Quasar, {
  plugins: { Notify, Dialog },
  config: {
    notify: {
      position: 'top-right',
      timeout: 3500,
      actions: [{ label: 'Dismiss', color: 'white' }],
    },
  },
})

app.use(createPinia())
app.use(router)

app.mount('#app')
