import http from './http'

export const fetchOrders = async (params) => {
  const { data } = await http.get('/api/orders', { params })
  return data
}

export const fetchAllOrders = async () => {
  const orders = []
  let page = 1
  const pageSize = 100
  let totalCount = null
  let safety = 0

  while (safety < 50) {
    const data = await fetchOrders({ page, pageSize })
    if (Array.isArray(data)) return data

    const items = data?.items || []
    totalCount = data?.totalCount ?? totalCount ?? items.length
    orders.push(...items)

    if (orders.length >= totalCount || items.length === 0) break
    page += 1
    safety += 1
  }

  return orders
}

export const fetchOrder = async (id) => {
  const { data } = await http.get(`/api/orders/${id}`)
  return data
}

export const createOrder = async (payload) => {
  const { data } = await http.post('/api/orders', payload)
  return data
}

export const updateOrder = async (id, payload) => {
  const { data } = await http.put(`/api/orders/${id}`, payload)
  return data
}

export const deleteOrder = async (id) => {
  await http.delete(`/api/orders/${id}`)
}

export const fetchOrderDetails = async (orderId, params, options = {}) => {
  const { silent = false } = options
  const { data } = await http.get(`/api/orders/${orderId}/details`, {
    params,
    meta: { silent },
  })
  return data
}

export const createOrderDetail = async (orderId, payload, options = {}) => {
  const { silent = false } = options
  const { data } = await http.post(`/api/orders/${orderId}/details`, payload, {
    meta: { silent },
  })
  return data
}

export const updateOrderDetail = async (orderId, productId, payload, options = {}) => {
  const { silent = false } = options
  const { data } = await http.put(`/api/orders/${orderId}/details/${productId}`, payload, {
    meta: { silent },
  })
  return data
}

export const deleteOrderDetail = async (orderId, productId, options = {}) => {
  const { silent = false } = options
  await http.delete(`/api/orders/${orderId}/details/${productId}`, {
    meta: { silent },
  })
}
