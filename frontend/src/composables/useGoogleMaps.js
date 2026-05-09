import { ref } from 'vue'

const mapsPromise = ref(null)

const loadGoogleMaps = (apiKey) => {
  if (!apiKey) return Promise.reject(new Error('Missing Google Maps API key'))
  if (mapsPromise.value) return mapsPromise.value

  mapsPromise.value = new Promise((resolve, reject) => {
    const existing = document.getElementById('google-maps-sdk')
    if (existing) {
      existing.addEventListener('load', () => resolve(window.google?.maps))
      existing.addEventListener('error', () => reject(new Error('Maps SDK failed to load')))
      if (window.google?.maps) resolve(window.google.maps)
      return
    }

    const script = document.createElement('script')
    script.id = 'google-maps-sdk'
    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=places`
    script.async = true
    script.defer = true
    script.onload = () => resolve(window.google?.maps)
    script.onerror = () => reject(new Error('Maps SDK failed to load'))
    document.head.appendChild(script)
  })

  return mapsPromise.value
}

export const useGoogleMaps = () => {
  return { loadGoogleMaps }
}
