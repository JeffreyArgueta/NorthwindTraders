import http from './http'

export const fetchCustomers = async (params) => {
  const { data } = await http.get('/api/customers', { params })
  return data
}

export const fetchAllCustomers = async () => {
  const customers = []
  let page = 1
  const pageSize = 100
  let totalCount = null
  let safety = 0

  while (safety < 50) {
    const data = await fetchCustomers({ page, pageSize })
    if (Array.isArray(data)) return data

    const items = data?.items || []
    totalCount = data?.totalCount ?? totalCount ?? items.length
    customers.push(...items)

    if (customers.length >= totalCount || items.length === 0) break
    page += 1
    safety += 1
  }

  return customers
}

export const fetchEmployees = async (params) => {
  const { data } = await http.get('/api/employees', { params })
  return data
}

export const fetchProducts = async (params) => {
  const { data } = await http.get('/api/products', { params })
  return data
}

export const fetchAllProducts = async () => {
  const products = []
  let page = 1
  const pageSize = 100
  let totalCount = null
  let safety = 0

  while (safety < 50) {
    const data = await fetchProducts({ page, pageSize })
    if (Array.isArray(data)) return data

    const items = data?.items || []
    totalCount = data?.totalCount ?? totalCount ?? items.length
    products.push(...items)

    if (products.length >= totalCount || items.length === 0) break
    page += 1
    safety += 1
  }

  return products
}

export const fetchShippers = async (params) => {
  const { data } = await http.get('/api/shippers', { params })
  return data
}

export const fetchCategories = async (params) => {
  const { data } = await http.get('/api/categories', { params })
  return data
}

export const fetchAllCategories = async () => {
  const categories = []
  let page = 1
  const pageSize = 100
  let totalCount = null
  let safety = 0

  while (safety < 50) {
    const data = await fetchCategories({ page, pageSize })
    if (Array.isArray(data)) return data

    const items = data?.items || []
    totalCount = data?.totalCount ?? totalCount ?? items.length
    categories.push(...items)

    if (categories.length >= totalCount || items.length === 0) break
    page += 1
    safety += 1
  }

  return categories
}
