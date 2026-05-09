/**
 * Calls Google Address Validation API directly via fetch.
 * Avoids the project axios instance (which has baseURL, auth interceptors)
 * and handles Google's specific error response format.
 */
export const validateAddress = async (payload, apiKey) => {
  const url = `https://addressvalidation.googleapis.com/v1:validateAddress?key=${apiKey}`

  const response = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })

  if (!response.ok) {
    let message = `Address validation failed (${response.status})`
    try {
      const errorBody = await response.json()
      if (errorBody?.error?.message) {
        message = errorBody.error.message
      }
    } catch {
      // ignore json parse error, use default message
    }
    throw new Error(message)
  }

  return response.json()
}
