import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import { quasar, transformAssetUrls } from '@quasar/vite-plugin'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue({ template: { transformAssetUrls } }),
    vueDevTools(),
    quasar({ sassVariables: 'src/quasar-variables.sass' })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      // Quasar's SASS imports use the literal 'src/...' path inside
      // node_modules/quasar. Make sure SASS can resolve that by
      // aliasing `src` to the project's src directory as well.
      'src': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
})
