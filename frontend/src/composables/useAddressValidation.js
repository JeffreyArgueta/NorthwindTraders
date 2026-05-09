import { computed, ref } from 'vue'
import { validateAddress } from '../api/addressValidation'

const normalizeRegionCode = (value) => {
  if (!value) return undefined
  const raw = String(value).trim()
  if (!raw) return undefined
  const upper = raw.toUpperCase().replace(/[^A-Z]/g, '')
  const mapping = {
    USA: 'US',
    US: 'US',
    'UNITED STATES': 'US',
    UK: 'GB',
    'UNITED KINGDOM': 'GB',
    GBR: 'GB',
    CANADA: 'CA',
    CA: 'CA',
  }
  if (mapping[upper]) return mapping[upper]
  if (upper.length === 2) return upper
  return undefined
}

const toAddressPayload = (form) => {
  const regionCode =
    normalizeRegionCode(form.shipCountry) ||
    String(form.shipCountry || '')
      .trim()
      .toUpperCase()
      .slice(0, 2)

  return {
    address: {
      locality: form.shipCity || undefined,
      regionCode,
    },
  }
}

export const useAddressValidation = () => {
  const validating = ref(false)
  const validatedAddress = ref(null)
  const geocodedLocation = ref(null)
  const validationErrors = ref([])

  const hasValidatedAddress = computed(() => !!validatedAddress.value)

  const runValidation = async (form, apiKey) => {
    if (!form.shipCity || !String(form.shipCountry || '').trim()) {
      validationErrors.value = ['City and country are required to validate.']
      return null
    }

    validating.value = true
    validationErrors.value = []

    try {
      const response = await validateAddress(toAddressPayload(form), apiKey)
      const result = response?.result
      const postal = result?.address?.postalAddress
      const location = result?.geocode?.location

      validatedAddress.value = {
        addressLines: postal?.addressLines || [],
        locality: postal?.locality,
        administrativeArea: postal?.administrativeArea,
        postalCode: postal?.postalCode,
        regionCode: postal?.regionCode,
        formattedAddress: result?.address?.formattedAddress,
      }

      geocodedLocation.value = location
        ? { lat: location.latitude, lng: location.longitude }
        : null

      if (!result?.verdict?.addressComplete) {
        validationErrors.value = [
          'Address could not be fully validated. Some fields may be inaccurate.',
        ]
      }

      return response
    } catch (error) {
      validationErrors.value = [error.message || 'Address validation failed.']
      throw error
    } finally {
      validating.value = false
    }
  }

  /**
   * Applies the validated city and country back to the form object.
   */
  const applyValidatedAddress = (form) => {
    if (!validatedAddress.value) return

    const addr = validatedAddress.value

    if (addr.locality) {
      form.shipCity = addr.locality
    }
    if (addr.regionCode) {
      form.shipCountry = addr.regionCode
    }
  }

  const clearValidation = () => {
    validatedAddress.value = null
    geocodedLocation.value = null
    validationErrors.value = []
  }

  return {
    validating,
    validatedAddress,
    geocodedLocation,
    validationErrors,
    hasValidatedAddress,
    runValidation,
    applyValidatedAddress,
    clearValidation,
  }
}
