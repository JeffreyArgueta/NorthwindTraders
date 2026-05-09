<script setup>
import { onMounted, ref, watch } from 'vue'
import { useGoogleMaps } from '../composables/useGoogleMaps'

const props = defineProps({
  apiKey: { type: String, required: true },
  center: { type: Object, default: () => ({ lat: 37.7749, lng: -122.4194 }) },
  marker: { type: Object, default: null },
  addressLabel: { type: String, default: '' },
  height: { type: String, default: '320px' },
})

const mapRef = ref(null)
const mapInstance = ref(null)
const markerInstance = ref(null)
const infoWindow = ref(null)
const { loadGoogleMaps } = useGoogleMaps()

const initMap = async () => {
  const maps = await loadGoogleMaps(props.apiKey)
  mapInstance.value = new maps.Map(mapRef.value, {
    center: props.center,
    zoom: 8,
    mapTypeControl: false,
    streetViewControl: false,
    fullscreenControl: false,
  })

  if (props.marker) {
    markerInstance.value = new maps.Marker({ position: props.marker, map: mapInstance.value })
  }
  infoWindow.value = new maps.InfoWindow()
}

const updateMarker = (location) => {
  if (!mapInstance.value || !window.google?.maps) return
  if (!location) {
    markerInstance.value?.setMap(null)
    markerInstance.value = null
    infoWindow.value?.close()
    return
  }
  if (!markerInstance.value) {
    markerInstance.value = new window.google.maps.Marker({
      position: location,
      map: mapInstance.value,
    })
  } else {
    markerInstance.value.setPosition(location)
  }
  if (props.addressLabel) {
    infoWindow.value?.setContent(`<div style="font-size:12px;">${props.addressLabel}</div>`)
    infoWindow.value?.open({ anchor: markerInstance.value, map: mapInstance.value })
  }
  mapInstance.value.panTo(location)
}

const updateAddressLabel = (label) => {
  if (!infoWindow.value || !markerInstance.value || !label) return
  infoWindow.value.setContent(`<div style="font-size:12px;">${label}</div>`)
  infoWindow.value.open({ anchor: markerInstance.value, map: mapInstance.value })
}

onMounted(() => {
  initMap()
})

watch(
  () => props.marker,
  (value) => {
    updateMarker(value)
  },
)

watch(
  () => props.addressLabel,
  (value) => {
    updateAddressLabel(value)
  },
)
</script>

<template>
  <div :style="{ width: '100%', height }" ref="mapRef" class="rounded-borders" />
</template>
