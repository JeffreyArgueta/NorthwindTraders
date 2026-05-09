<script setup>
import { computed, ref } from 'vue'
import { fetchAllOrders, fetchOrderDetails } from '../api/orders'
import { fetchAllCategories, fetchAllCustomers, fetchAllProducts } from '../api/lookups'
import { exportToCsv } from '../utils/exporters'
import { buildOrderReportData } from '../utils/reporting'
import { exportOrderReportPdf } from '../utils/pdf'

const loading = ref(false)
const orders = ref([])
const pagination = ref({ page: 1, rowsPerPage: 25 })
const filterYear = ref('all')
const filterPeriod = ref('month')
const filterRegion = ref('')

const columns = [
  { name: 'orderId', label: 'Order #', field: 'orderID', align: 'left' },
  {
    name: 'customer',
    label: 'Customer',
    field: (row) => row.customer?.companyName || row.customerName,
    align: 'left',
  },
  { name: 'orderDate', label: 'Order Date', field: 'orderDate', align: 'left' },
  {
    name: 'region',
    label: 'Region',
    field: (row) => row.shipRegion || row.shipCountry || row.customer?.region || row.customer?.country,
    align: 'left',
  },
  { name: 'products', label: 'Products', field: (row) => row.orderDetails?.length || 0, align: 'right' },
  {
    name: 'total',
    label: 'Total',
    field: (row) => {
      const itemsTotal = (row.orderDetails || []).reduce((sum, detail) => {
        const quantity = Number(detail.quantity || 0)
        const unitPrice = Number(detail.unitPrice || 0)
        const discount = Number(detail.discount || 0)
        return sum + unitPrice * quantity * (1 - discount)
      }, 0)
      return (itemsTotal + Number(row.freight || 0)).toFixed(2)
    },
    align: 'right',
  },
  { name: 'actions', label: 'Actions', field: 'actions', align: 'right' },
]

const loadOrders = async () => {
  loading.value = true
  try {
    const [response, customers, products, categories] = await Promise.all([
      fetchAllOrders(),
      fetchAllCustomers(),
      fetchAllProducts(),
      fetchAllCategories(),
    ])
    const customerMap = new Map(customers.map((customer) => [customer.customerID, customer]))
    const productMap = new Map(products.map((product) => [product.productID, product]))
    const categoryMap = new Map(categories.map((category) => [category.categoryID, category]))
    const items = response?.items || response || []
    orders.value = items.map((order) => ({
      ...order,
      customer: order.customer || customerMap.get(order.customerID) || null,
      customerName:
        order.customer?.companyName || customerMap.get(order.customerID)?.companyName || 'Unknown',
    }))

    await loadOrderDetails(orders.value, productMap, categoryMap)
  } finally {
    loading.value = false
  }
}

const loadOrderDetails = async (list, productMap, categoryMap) => {
  const responses = await Promise.allSettled(
    list.map((order) => fetchOrderDetails(order.orderID, { page: 1, pageSize: 200 }, { silent: true })),
  )

  orders.value = list.map((order, index) => {
    const response = responses[index]
    if (response.status !== 'fulfilled') return order
    const details = response.value?.items || response.value || []
    const enriched = details.map((detail) => {
      const product = productMap?.get(detail.productID) || null
      const category = product ? categoryMap?.get(product.categoryID) || null : null
      return {
        ...detail,
        product,
        category,
      }
    })
    return { ...order, orderDetails: enriched }
  })
}

const yearOptions = computed(() => {
  const years = Array.from(
    new Set(
      orders.value
        .map((order) => (order.orderDate ? new Date(order.orderDate).getFullYear() : null))
        .filter(Boolean),
    ),
  ).sort((a, b) => b - a)

  return [{ label: 'All Years', value: 'all' }, ...years.map((year) => ({ label: year, value: year }))]
})

const filteredOrders = computed(() => {
  return orders.value.filter((order) => {
    const date = order.orderDate ? new Date(order.orderDate) : null
    if (!date) return false
    if (filterYear.value !== 'all' && date.getFullYear() !== Number(filterYear.value)) return false
    const regionValue = order.shipRegion || order.shipCountry || order.customer?.region || order.customer?.country
    if (filterRegion.value && regionValue !== filterRegion.value) return false
    return true
  })
})

const regions = computed(() => {
  const set = new Set(
    orders.value
      .map((order) => order.shipRegion || order.shipCountry || order.customer?.region || order.customer?.country)
      .filter(Boolean),
  )
  return Array.from(set)
})

const metrics = computed(() => {
  const summary = {}
  filteredOrders.value.forEach((order) => {
    const date = new Date(order.orderDate)
    let key = ''
    if (filterPeriod.value === 'month') {
      key = `${date.getMonth() + 1}/${date.getFullYear()}`
    } else if (filterPeriod.value === 'week') {
      const week = Math.ceil(date.getDate() / 7)
      key = `W${week} ${date.toLocaleString('default', { month: 'short' })}`
    } else {
      key = `${date.getFullYear()}`
    }
    summary[key] = (summary[key] || 0) + 1
  })
  return summary
})

const shipmentByRegion = computed(() => {
  const summary = {}
  filteredOrders.value.forEach((order) => {
    const key =
      order.shipRegion || order.shipCountry || order.customer?.region || order.customer?.country || 'Unknown'
    summary[key] = (summary[key] || 0) + 1
  })
  return summary
})

const maxMetric = computed(() => {
  const values = Object.values(metrics.value)
  return values.length ? Math.max(...values) : 1
})

const maxShipment = computed(() => {
  const values = Object.values(shipmentByRegion.value)
  return values.length ? Math.max(...values) : 1
})

const orderRows = computed(() => filteredOrders.value)

const exportExcel = () => {
  const rows = [
    ['Order #', 'Customer', 'Order Date', 'Region', 'Products', 'Total'],
    ...orderRows.value.map((row) => [
      row.orderID,
      row.customer?.companyName || '',
      row.orderDate,
      row.shipRegion || '',
      row.orderDetails?.length || 0,
      ((row.orderDetails || []).reduce((sum, detail) => {
        const quantity = Number(detail.quantity || 0)
        const unitPrice = Number(detail.unitPrice || 0)
        const discount = Number(detail.discount || 0)
        return sum + unitPrice * quantity * (1 - discount)
      }, 0) + Number(row.freight || 0)).toFixed(2),
    ]),
  ]
  exportToCsv('orders-report.csv', rows)
}

const exportReportPdf = () => {
  const first = orderRows.value[0]
  if (!first) return
  try {
    exportSelectedOrderPdf(first)
  } catch (error) {
    console.error('PDF export failed', error)
  }
}

const exportSelectedOrderPdf = (order) => {
  const lineItems = (order.orderDetails || []).map((detail) => {
    const quantity = Number(detail.quantity || 0)
    const unitPrice = Number(detail.unitPrice || 0)
    const discount = Number(detail.discount || 0)
    const lineTotal = unitPrice * quantity * (1 - discount)
    return { detail, lineTotal, quantity, unitPrice, discount }
  })
  const itemsTotal = lineItems.reduce((sum, item) => sum + item.lineTotal, 0)
  const freightTotal = Number(order.freight || 0)
  const orderTotal = Number((itemsTotal + freightTotal).toFixed(2))

  const meta = {
    orderId: order.orderID,
    orderDate: order.orderDate,
    customer: order.customer?.companyName || '',
    freight: order.freight,
    total: orderTotal,
  }
  const items = lineItems.map(({ detail, lineTotal, quantity, unitPrice, discount }) => ({
    productId: detail.productID,
    productName: detail.product?.productName || 'Unknown',
    category: detail.category?.categoryName || 'Unknown',
    quantity,
    unitPrice: Number(unitPrice.toFixed(2)),
    discount: Number(discount.toFixed(2)),
    total: Number(lineTotal.toFixed(2)),
  }))
  const report = buildOrderReportData({ meta, items })
  try {
    exportOrderReportPdf(`order-${order.orderID}.pdf`, report)
  } catch (error) {
    console.error('PDF export failed', error)
  }
}

loadOrders()
</script>

<template>
  <q-page class="q-pa-lg app-page">
    <div class="q-mb-md">
      <div class="text-h5 section-title">Reporting</div>
      <div class="text-body2 subtle-text">Monitor performance and export insights.</div>
    </div>

    <q-card class="q-mb-md card-surface">
      <q-card-section class="row q-col-gutter-md">
        <div class="col-12 col-md-3">
          <q-select
            v-model="filterYear"
            :options="yearOptions"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            label="Year"
            dense
            outlined
            clearable
          />
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="filterPeriod"
            :options="[
              { label: 'Month', value: 'month' },
              { label: 'Week', value: 'week' },
              { label: 'Year', value: 'year' },
            ]"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            label="Period"
            dense
            outlined
          />
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="filterRegion"
            :options="regions"
            label="Region"
            dense
            outlined
            clearable
          />
        </div>
      </q-card-section>
    </q-card>

    <div class="row q-col-gutter-md q-mb-md">
      <div class="col-12 col-md-6">
        <q-card class="card-surface">
          <q-card-section>
            <div class="text-subtitle1 section-title">Orders per {{ filterPeriod }}</div>
            <div class="q-mt-md">
              <div v-for="(value, label) in metrics" :key="label" class="row items-center q-mb-sm">
                <div class="col-4 text-caption text-grey-7">{{ label }}</div>
                <div class="col-8">
                  <q-linear-progress :value="value / maxMetric" color="primary" />
                </div>
              </div>
            </div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-12 col-md-6">
        <q-card class="card-surface">
          <q-card-section>
            <div class="text-subtitle1 section-title">Shipments by Region</div>
            <div class="q-mt-md">
              <div
                v-for="(value, region) in shipmentByRegion"
                :key="region"
                class="row items-center q-mb-sm"
              >
                <div class="col-4 text-caption text-grey-7">{{ region }}</div>
                <div class="col-8">
                  <q-linear-progress :value="value / maxShipment" color="secondary" />
                </div>
              </div>
            </div>
          </q-card-section>
        </q-card>
      </div>
    </div>

    <q-card class="card-surface">
      <q-card-section class="row items-center q-pb-none">
        <div class="text-subtitle1 section-title">Order Details</div>
        <q-space />
        <div class="table-toolbar">
          <q-btn flat icon="download" label="Export Excel" @click="exportExcel" />
          <q-btn flat icon="picture_as_pdf" label="Export PDF" @click="exportReportPdf" />
        </div>
      </q-card-section>
      <q-card-section>
        <q-table
          :rows="orderRows"
          :columns="columns"
          row-key="orderID"
          :loading="loading"
          v-model:pagination="pagination"
          :rows-per-page-options="[10, 25, 50, 100]"
          flat
          bordered
          separator="horizontal"
          aria-label="Order report table"
        >
          <template #body-cell-actions="props">
            <q-td :props="props">
              <q-btn
                flat
                icon="picture_as_pdf"
                label="PDF"
                @click="exportSelectedOrderPdf(props.row)"
              />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>
  </q-page>
</template>
