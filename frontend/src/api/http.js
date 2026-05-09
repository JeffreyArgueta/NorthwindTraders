import axios from 'axios'
import { Notify } from 'quasar'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 15000,
})

const getErrorMessage = (error) => {
  if (!error.response) return 'Network error. Please check your connection.'
  const { status, data } = error.response
  if (data?.title) return data.title
  if (status === 400) return 'Validation error. Please review the form.'
  if (status === 404) return 'Resource not found.'
  if (status >= 500) return 'Server error. Please try again later.'
  return 'Unexpected error. Please retry.'
}

// Response interceptor — global error handling
http.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.config?.meta?.silent) return Promise.reject(error)
    Notify.create({
      type: 'negative',
      message: getErrorMessage(error),
    })
    return Promise.reject(error)
  },
)

export const setAuthHeader = (token) => {
  if (!token) {
    delete http.defaults.headers.common.Authorization
    return
  }
  http.defaults.headers.common.Authorization = `Bearer ${token}`
}

export default http
