import { describe, expect, it, vi } from 'vitest'
import { useAddressValidation } from '../../composables/useAddressValidation'
import * as addressApi from '../../api/addressValidation'

describe('useAddressValidation', () => {
  it('returns validation errors when required fields missing', async () => {
    const { runValidation, validationErrors } = useAddressValidation()
    await runValidation({ shipAddress: '', shipCity: '', shipCountry: '' }, 'key')
    expect(validationErrors.value.length).toBeGreaterThan(0)
  })

  it('stores validated address and geocode', async () => {
    vi.spyOn(addressApi, 'validateAddress').mockResolvedValue({
      result: {
        verdict: { addressComplete: true },
        address: {
          formattedAddress: '1 Test St, City',
          postalAddress: {
            addressLines: ['1 Test St'],
            locality: 'City',
            administrativeArea: 'CA',
            postalCode: '12345',
            regionCode: 'US',
          },
        },
        geocode: { location: { latitude: 10, longitude: 20 } },
      },
    })

    const { runValidation, validatedAddress, geocodedLocation } = useAddressValidation()
    await runValidation({
      shipAddress: '1 Test St',
      shipCity: 'City',
      shipCountry: 'US',
    }, 'key')

    expect(validatedAddress.value.formattedAddress).toBe('1 Test St, City')
    expect(geocodedLocation.value).toEqual({ lat: 10, lng: 20 })
  })
})
