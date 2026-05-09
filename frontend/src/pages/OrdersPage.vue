<script setup>
import { computed, ref } from 'vue'
import { useQuasar } from 'quasar'
import OrderForm from '../components/OrderForm.vue'
import { deleteOrder, deleteOrderDetail, fetchOrderDetails, fetchOrders } from '../api/orders'
import { fetchAllCustomers } from '../api/lookups'

const $q = useQuasar()
const loading = ref(false)
const orders = ref([])
const allCustomers = ref([])
const pagination = ref({ page: 1, rowsPerPage: 25, rowsNumber: 0 })
const showForm = ref(false)
const selectedOrder = ref(null)
const searchQuery = ref('')

const googleApiKey = import.meta.env.VITE_GOOGLE_MAPS_API_KEY || ''

const columns = [
  { name: 'orderId', label: 'Order #', field: 'orderID', align: 'left' },
  {
    name: 'customer',
    label: 'Customer',
    field: (row) => row.customer?.companyName || row.customerName,
    align: 'left',
  },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', align: 'left' },
  { name: 'shipTo', label: 'Ship To', field: 'shipAddress', align: 'left' },
  {
    name: 'shipRegion',
    label: 'Region',
    field: (row) => row.shipRegion || row.shipCountry || row.customer?.region || row.customer?.country,
    align: 'left',
  },
  { name: 'freight', label: 'Freight', field: 'freight', align: 'right' },
  { name: 'actions', label: 'Actions', field: 'actions', align: 'right' },
]

const loadOrders = async (props) => {
  if (props?.pagination) pagination.value = { ...pagination.value, ...props.pagination }
  loading.value = true
  try {
    const [response, customers] = await Promise.all([
      fetchOrders({
        page: pagination.value.page,
        pageSize: pagination.value.rowsPerPage,
      }),
      fetchAllCustomers(),
    ])

    allCustomers.value = customers
    const customerMap = new Map(customers.map((customer) => [customer.customerID, customer]))
    const items = response?.items || response || []
    orders.value = items.map((order) => ({
      ...order,
      customer: order.customer || customerMap.get(order.customerID) || null,
      customerName:
        order.customer?.companyName || customerMap.get(order.customerID)?.companyName || 'Unknown',
    }))
    pagination.value.rowsNumber = response?.totalCount || orders.value.length
  } finally {
    loading.value = false
  }
}

const startCreate = () => {
  selectedOrder.value = null
  showForm.value = true
}

const startEdit = async (order) => {
  loading.value = true
  try {
    const detailsResponse = await fetchOrderDetails(order.orderID, { page: 1, pageSize: 200 }, { silent: true })
    const details = detailsResponse?.items || detailsResponse || []
    selectedOrder.value = { ...order, orderDetails: details }
    showForm.value = true
  } finally {
    loading.value = false
  }
}

const confirmDelete = (order) => {
  $q.dialog({
    title: 'Delete Order',
    message: `Delete order #${order.orderID}?`,
    cancel: true,
    persistent: true,
  }).onOk(async () => {
    try {
      const detailsResponse = await fetchOrderDetails(order.orderID, { page: 1, pageSize: 200 }, { silent: true })
      const details = detailsResponse?.items || detailsResponse || []
      await Promise.allSettled(
        details.map((detail) => deleteOrderDetail(order.orderID, detail.productID, { silent: true })),
      )
      await deleteOrder(order.orderID)
      $q.notify({ type: 'positive', message: 'Order deleted.' })
      await loadOrders({ pagination: pagination.value })
    } catch (error) {
      $q.notify({ type: 'negative', message: 'Failed to delete order.' })
    }
  })
}

const onSaved = () => {
  showForm.value = false
  loadOrders()
}



const tableRows = computed(() => {
  const query = (searchQuery.value || '').trim().toLowerCase()
  return orders.value.filter((order) => {
    if (!query) return true
    return (
      String(order.orderID).includes(query) ||
      order.shipName?.toLowerCase().includes(query) ||
      order.customerName?.toLowerCase().includes(query)
    )
  })
})

loadOrders()
</script>

<template>
  <q-page class="q-pa-lg app-page">
    <div class="row items-center q-mb-md">
      <div>
        <div class="text-h5 section-title">Order Management</div>
        <div class="text-body2 subtle-text">Create, update, and track customer orders.</div>
      </div>
      <q-space />
      <q-btn color="primary" icon="add" label="New Order" @click="startCreate" />
    </div>

    <q-card class="q-mb-lg card-surface">
      <q-card-section class="q-pa-md">
        <div class="row q-col-gutter-md q-mb-md">
          <div class="col-12">
            <q-input
              v-model="searchQuery"
              dense
              outlined
              placeholder="Search by order ID, customer, or ship name"
              label="Search"
              clearable
              @clear="searchQuery = ''"
            />
          </div>
        </div>
        <q-table
          :rows="tableRows"
          :columns="columns"
          row-key="orderID"
          :loading="loading"
          v-model:pagination="pagination"
          :rows-per-page-options="[10, 25, 50, 100]"
          @request="loadOrders"
          flat
          bordered
          separator="horizontal"
          aria-label="Orders table"
        >
          <template #body-cell-actions="props">
            <q-td :props="props">
              <q-btn flat icon="edit" color="primary" @click="startEdit(props.row)" aria-label="Edit order" />
              <q-btn flat icon="delete" color="negative" @click="confirmDelete(props.row)" aria-label="Delete order" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>

    <q-dialog v-model="showForm" persistent maximized>
      <q-card class="bg-grey-1">
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">{{ selectedOrder ? 'Edit Order' : 'New Order' }}</div>
          <q-space />
          <q-btn icon="close" flat round dense @click="showForm = false" />
        </q-card-section>
        <q-card-section>
          <OrderForm
            :mode="selectedOrder ? 'edit' : 'create'"
            :initial-order="selectedOrder"
            :api-key="googleApiKey"
            @saved="onSaved"
            @cancel="showForm = false"
          />
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>
