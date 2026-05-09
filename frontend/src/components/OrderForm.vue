<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { fetchCustomers, fetchEmployees, fetchProducts, fetchShippers } from '../api/lookups'
import {
  createOrder,
  createOrderDetail,
  deleteOrderDetail,
  updateOrder,
  updateOrderDetail,
} from '../api/orders'
import GoogleMapEmbed from './GoogleMapEmbed.vue'
import { useAddressValidation } from '../composables/useAddressValidation'

const props = defineProps({
  mode: { type: String, default: 'create' },
  initialOrder: { type: Object, default: () => ({}) },
  apiKey: { type: String, default: '' },
})

const emit = defineEmits(['saved', 'cancel'])
const $q = useQuasar()
const formRef = ref(null)
const fieldErrors = ref({})
const baselineOrderDetails = ref([])
const savingItems = ref(false)

const getOrderId = () =>
  props.initialOrder?.orderId ||
  props.initialOrder?.orderID ||
  props.initialOrder?.OrderID ||
  props.initialOrder?.id ||
  null

const form = reactive({
  customerId: null,
  employeeId: null,
  orderDate: null,
  requiredDate: null,
  shippedDate: null,
  shipperId: null,
  freight: 0,
  shipName: '',
  shipAddress: '',
  shipCity: '',
  shipRegion: '',
  shipPostalCode: '',
  shipCountry: '',
  orderDetails: [],
})

const loading = ref(false)
const customers = ref([])
const employees = ref([])
const products = ref([])
const shippers = ref([])

const {
  validating,
  validatedAddress,
  geocodedLocation,
  validationErrors,
  hasValidatedAddress,
  runValidation,
  applyValidatedAddress,
  clearValidation,
} = useAddressValidation()

const toDateInput = (value) => {
  if (!value) return null
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return null
  return date.toISOString().slice(0, 10)
}

const applyInitialOrder = () => {
  if (!props.initialOrder) return
  const normalizedDetails = (props.initialOrder.orderDetails || []).map((detail) => {
    const productId =
      detail.productId ?? (detail.productID === 0 ? 0 : detail.productID) ?? detail.productId
    return {
      ...detail,
      productId: productId !== undefined && productId !== null ? Number(productId) : null,
      unitPrice: detail.unitPrice ?? detail.UnitPrice ?? detail.unitprice ?? detail.unit_price ?? detail.unitPrice,
      quantity: detail.quantity ?? detail.Quantity ?? detail.quantity,
      discount: detail.discount ?? detail.Discount ?? detail.discount,
    }
  })

  Object.assign(form, {
    customerId: props.initialOrder.customerId || props.initialOrder.customerID || null,
    employeeId: props.initialOrder.employeeId || props.initialOrder.employeeID || null,
    orderDate: toDateInput(props.initialOrder.orderDate),
    requiredDate: toDateInput(props.initialOrder.requiredDate),
    shippedDate: toDateInput(props.initialOrder.shippedDate),
    shipperId: props.initialOrder.shipVia || null,
    freight: props.initialOrder.freight || 0,
    shipName: props.initialOrder.shipName || '',
    shipAddress: props.initialOrder.shipAddress || '',
    shipCity: props.initialOrder.shipCity || '',
    shipRegion: props.initialOrder.shipRegion || '',
    shipPostalCode: props.initialOrder.shipPostalCode || '',
    shipCountry: props.initialOrder.shipCountry || '',
    orderDetails: normalizedDetails,
  })
  baselineOrderDetails.value = normalizedDetails.map((detail) => ({ ...detail }))
  fieldErrors.value = {}
}

const loadLookups = async () => {
  loading.value = true
  try {
    const [customersData, employeesData, productsData, shippersData] = await Promise.all([
      fetchCustomers({ pageSize: 100 }),
      fetchEmployees({ pageSize: 100 }),
      fetchProducts({ pageSize: 100 }),
      fetchShippers({ pageSize: 100 }),
    ])
    customers.value = customersData?.items || customersData || []
    employees.value = employeesData?.items || employeesData || []
    products.value = productsData?.items || productsData || []
    shippers.value = shippersData?.items || shippersData || []
  } finally {
    loading.value = false
  }
}

const addLineItem = () => {
  form.orderDetails.push({ productId: null, unitPrice: 0, quantity: 1, discount: 0 })
}

const removeLineItem = (index) => {
  form.orderDetails.splice(index, 1)
}

const toPayload = (includeDetails = true) => {
  const payload = {
    customerID: typeof form.customerId === 'string' ? form.customerId : form.customerId?.customerID || null,
    employeeID: Number(form.employeeId ?? form.employeeId?.employeeID ?? null),
    orderDate: form.orderDate,
    requiredDate: form.requiredDate,
    shippedDate: form.shippedDate,
    shipVia: Number(form.shipperId ?? form.shipperId?.shipperID ?? null),
    freight: Number(form.freight || 0),
    shipName: form.shipName,
    shipAddress: form.shipAddress,
    shipCity: form.shipCity,
    shipRegion: form.shipRegion,
    shipPostalCode: form.shipPostalCode,
    shipCountry: form.shipCountry,
  }
  if (!includeDetails) return payload
  return {
    ...payload,
    orderDetails: form.orderDetails.map((item) => ({
      productID: item.productId,
      unitPrice: Number(item.unitPrice || 0),
      quantity: Number(item.quantity || 0),
      discount: Number(item.discount || 0),
    })),
  }
}

const getProductId = (detail) => {
  const raw = detail?.productId ?? detail?.productID ?? detail?.ProductID ?? detail?.ProductId
  const value = Number(raw)
  return Number.isNaN(value) ? null : value
}

const syncOrderDetails = async (orderId, baselineDetails) => {
  const original = baselineDetails || []
  const originalMap = new Map(original.map((detail) => [getProductId(detail), detail]))
  const currentMap = new Map(form.orderDetails.map((item) => [getProductId(item), item]))

  const creates = []
  const updates = []
  const deletes = []

  currentMap.forEach((item, productId) => {
    if (productId === null) return
    if (!originalMap.has(productId)) {
      creates.push(
        createOrderDetail(
          orderId,
          {
            productID: productId,
            unitPrice: Number(item.unitPrice || 0),
            quantity: Number(item.quantity || 0),
            discount: Number(item.discount || 0),
          },
          { silent: true },
        ),
      )
    } else {
      updates.push(
        updateOrderDetail(
          orderId,
          productId,
          {
            unitPrice: Number(item.unitPrice || 0),
            quantity: Number(item.quantity || 0),
            discount: Number(item.discount || 0),
          },
          { silent: true },
        ),
      )
    }
  })

  originalMap.forEach((_detail, productId) => {
    if (productId === null) return
    if (!currentMap.has(productId)) {
      deletes.push(deleteOrderDetail(orderId, productId, { silent: true }))
    }
  })

  const results = await Promise.allSettled([...creates, ...updates, ...deletes])
  const failed = results.filter((result) => result.status === 'rejected')
  if (failed.length) {
    $q.notify({
      type: 'negative',
      message: 'Some order line items failed to save. Please retry.',
    })
  }
}

const createOrderDetails = async (orderId) => {
  const creates = form.orderDetails
    .map((item) => ({
      productId: getProductId(item),
      unitPrice: Number(item.unitPrice || 0),
      quantity: Number(item.quantity || 0),
      discount: Number(item.discount || 0),
    }))
    .filter((item) => item.productId !== null)
    .map((item) =>
      createOrderDetail(
        orderId,
        {
          productID: item.productId,
          unitPrice: item.unitPrice,
          quantity: item.quantity,
          discount: item.discount,
        },
        { silent: true },
      ),
    )

  if (!creates.length) return
  const results = await Promise.allSettled(creates)
  const failed = results.filter((result) => result.status === 'rejected')
  if (failed.length) {
    throw new Error('order-details-create-failed')
  }
}

const saveOrder = async () => {
  const isValid = await formRef.value?.validate()
  if (isValid === false) {
    $q.notify({ type: 'warning', message: 'Please fix the highlighted fields.' })
    return
  }
  loading.value = true
  try {
    const payload = toPayload(false)
    const orderId = getOrderId()
    if (props.mode === 'edit' && !orderId) {
      $q.notify({ type: 'negative', message: 'Order ID missing. Unable to update.' })
      return
    }
    const response =
      props.mode === 'edit' ? await updateOrder(orderId, payload) : await createOrder(payload)
    const createdOrderId = response?.orderID || response?.orderId || response?.OrderID || orderId
    if (!createdOrderId) {
      $q.notify({ type: 'negative', message: 'Order saved without an ID. Unable to save line items.' })
      return
    }
    if (props.mode === 'edit') {
      await syncOrderDetails(createdOrderId, baselineOrderDetails.value)
    } else {
      await createOrderDetails(createdOrderId)
    }
    emit('saved', response)
    $q.notify({ type: 'positive', message: 'Order saved.' })
    fieldErrors.value = {}
  } catch (error) {
    if (error.response?.status === 400) {
      const errors = error.response.data?.errors || error.response.data?.Errors
      if (errors) fieldErrors.value = mapServerErrors(errors)
    }
    $q.notify({ type: 'negative', message: 'Failed to save order. Please try again.' })
  } finally {
    loading.value = false
  }
}

const saveLineItems = async () => {
  if (props.mode !== 'edit') return
  const orderId = getOrderId()
  if (!orderId) {
    $q.notify({ type: 'negative', message: 'Order ID missing. Unable to save items.' })
    return
  }
  const validItems = form.orderDetails.filter((item) => getProductId(item) !== null)
  if (!validItems.length) {
    $q.notify({ type: 'warning', message: 'Add at least one valid product before saving.' })
    return
  }
  savingItems.value = true
  try {
    await syncOrderDetails(orderId, baselineOrderDetails.value)
    baselineOrderDetails.value = form.orderDetails.map((detail) => ({ ...detail }))
    $q.notify({ type: 'positive', message: 'Order items saved.' })
  } catch (error) {
    $q.notify({ type: 'negative', message: 'Failed to save order items.' })
  } finally {
    savingItems.value = false
  }
}

const runAddressValidation = async () => {
  if (!props.apiKey) {
    $q.notify({ type: 'warning', message: 'Google Maps API key missing.' })
    return
  }

  try {
    await runValidation(form, props.apiKey)

    if (hasValidatedAddress.value) {
      // Auto-apply the validated/corrected address fields to the form
      applyValidatedAddress(form)

      if (validationErrors.value.length) {
        $q.notify({
          type: 'warning',
          message: 'Address partially validated — suggested corrections have been applied.',
        })
      } else {
        $q.notify({
          type: 'positive',
          message: 'Shipping address validated and corrected.',
        })
      }
    }
  } catch (error) {
    // error is already captured in validationErrors ref by the composable
    $q.notify({
      type: 'negative',
      message: error.message || 'Address validation failed. Please check the fields and try again.',
    })
  }
}

const availableProducts = computed(() =>
  products.value.map((product) => ({
    label: product.productName,
    value: Number(product.productID),
    unitPrice: product.unitPrice,
  }))
)

watch(
  () => form.orderDetails.map((item) => item.productId),
  () => {
    form.orderDetails.forEach((item) => {
      const selected = products.value.find((product) => product.productID === item.productId)
      if (selected && (!item.unitPrice || Number(item.unitPrice) === 0)) {
        item.unitPrice = selected.unitPrice
      }
    })
  },
)

const mapServerErrors = (errors) => {
  if (Array.isArray(errors)) {
    return errors.reduce((acc, err) => {
      acc[err.propertyName] = err.errorMessage
      return acc
    }, {})
  }
  return Object.entries(errors).reduce((acc, [key, value]) => {
    acc[key] = Array.isArray(value) ? value.join(', ') : String(value)
    return acc
  }, {})
}

const fieldError = (name) => fieldErrors.value[name] || ''

const requiredRule = (label) => (value) => (!!value && value !== '') || `${label} is required`
const nonNegativeRule = (label) => (value) => value === null || value === '' || Number(value) >= 0 || `${label} must be >= 0`
const quantityRule = (value) => Number(value) > 0 || 'Quantity must be greater than 0'
const maxDiscountRule = (value) => Number(value) <= 1 || 'Discount must be <= 1.0'

watch(
  () => props.initialOrder,
  () => {
    applyInitialOrder()
  },
  { immediate: true },
)

loadLookups()
</script>

<template>
  <q-card class="q-pa-lg card-surface">
    <q-form ref="formRef" class="q-gutter-md" @submit.prevent="saveOrder">
    <q-card-section class="row items-center q-pb-none">
      <div>
        <div class="text-h6 section-title">{{ mode === 'edit' ? 'Update Order' : 'Create Order' }}</div>
        <div class="text-body2 subtle-text">Complete the required fields before saving.</div>
      </div>
      <q-space />
      <q-btn flat label="Cancel" color="grey" @click="emit('cancel')" />
      <q-btn color="primary" label="Save" :loading="loading" type="submit" />
    </q-card-section>

    <q-separator class="q-my-md" />

    <q-card-section>
      <div class="row q-col-gutter-md">
        <div class="col-12 col-md-6">
          <q-select
            v-model="form.customerId"
            :options="customers"
            option-label="companyName"
            option-value="customerID"
            emit-value
            map-options
            label="Customer"
            dense
            outlined
            aria-label="Customer selection"
            :rules="[requiredRule('Customer')]"
            :error="!!fieldError('CustomerID')"
            :error-message="fieldError('CustomerID')"
          />
        </div>
        <div class="col-12 col-md-6">
          <q-select
            v-model="form.employeeId"
            :options="employees"
            option-label="lastName"
            option-value="employeeID"
            emit-value
            map-options
            label="Employee"
            dense
            outlined
            aria-label="Employee selection"
            :error="!!fieldError('EmployeeID')"
            :error-message="fieldError('EmployeeID')"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            v-model="form.orderDate"
            type="date"
            label="Order Date"
            dense
            outlined
            aria-label="Order date"
            :rules="[requiredRule('Order date')]"
            :error="!!fieldError('OrderDate')"
            :error-message="fieldError('OrderDate')"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input v-model="form.requiredDate" type="date" label="Required Date" dense outlined />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            v-model="form.shippedDate"
            type="date"
            label="Shipped Date"
            dense
            outlined
            aria-label="Shipped date"
            :error="!!fieldError('ShippedDate')"
            :error-message="fieldError('ShippedDate')"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-select
            v-model="form.shipperId"
            :options="shippers"
            option-label="companyName"
            option-value="shipperID"
            emit-value
            map-options
            label="Shipper"
            dense
            outlined
            aria-label="Shipper selection"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            v-model="form.freight"
            type="number"
            label="Freight"
            dense
            outlined
            aria-label="Freight charges"
            :rules="[nonNegativeRule('Freight')]"
            :error="!!fieldError('Freight')"
            :error-message="fieldError('Freight')"
          />
        </div>
        <div class="col-12 col-md-4">
          <q-input
            v-model="form.shipName"
            label="Ship Name"
            dense
            outlined
            aria-label="Ship name"
            :rules="[requiredRule('Ship name')]"
            :error="!!fieldError('ShipName')"
            :error-message="fieldError('ShipName')"
          />
        </div>
      </div>
    </q-card-section>

    <q-separator class="q-my-md" />

    <q-card-section>
      <div class="text-subtitle1 q-mb-sm section-title">Shipping Details</div>
      <div class="row q-col-gutter-md">
        <div class="col-12 col-md-6">
          <q-input
            v-model="form.shipAddress"
            label="Address"
            dense
            outlined
            aria-label="Shipping address"
            :rules="[requiredRule('Address')]"
            :error="!!fieldError('ShipAddress')"
            :error-message="fieldError('ShipAddress')"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-input
            v-model="form.shipCity"
            label="City"
            dense
            outlined
            aria-label="Shipping city"
            :rules="[requiredRule('City')]"
            :error="!!fieldError('ShipCity')"
            :error-message="fieldError('ShipCity')"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-input
            v-model="form.shipRegion"
            label="Region"
            dense
            outlined
            aria-label="Shipping region"
            :error="!!fieldError('ShipRegion')"
            :error-message="fieldError('ShipRegion')"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-input
            v-model="form.shipPostalCode"
            label="Postal Code"
            dense
            outlined
            aria-label="Shipping postal code"
            :error="!!fieldError('ShipPostalCode')"
            :error-message="fieldError('ShipPostalCode')"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-input
            v-model="form.shipCountry"
            label="Country"
            dense
            outlined
            aria-label="Shipping country"
            :rules="[requiredRule('Country')]"
            :error="!!fieldError('ShipCountry')"
            :error-message="fieldError('ShipCountry')"
          />
        </div>
        <div class="col-12 col-md-3 flex items-center q-gutter-x-sm">
          <q-btn
            color="primary"
            label="Validate Address"
            class="full-width"
            :loading="validating"
            :disable="validating"
            @click="runAddressValidation"
            aria-label="Validate shipping address"
          />
          <q-btn
            v-if="hasValidatedAddress"
            flat
            dense
            color="grey"
            icon="close"
            size="sm"
            @click="clearValidation"
            aria-label="Clear address validation"
          >
            <q-tooltip>Clear validation result</q-tooltip>
          </q-btn>
        </div>
      </div>

      <div v-if="validationErrors.length" class="q-mt-md">
        <q-banner dense class="bg-orange-1 text-orange-10">
          <div v-for="error in validationErrors" :key="error">{{ error }}</div>
        </q-banner>
      </div>

      <div v-if="hasValidatedAddress" class="q-mt-md">
        <q-banner class="bg-green-1 text-green-10" dense>
          <div class="text-weight-bold">Validated Address:</div>
          <div class="q-mt-xs">{{ validatedAddress.formattedAddress }}</div>
          <div v-if="geocodedLocation" class="text-caption q-mt-xs text-grey-8">
            Lat: {{ geocodedLocation.lat?.toFixed(4) }}, Lng: {{ geocodedLocation.lng?.toFixed(4) }}
          </div>
        </q-banner>
      </div>

      <div class="q-mt-md">
        <GoogleMapEmbed
          v-if="apiKey"
          :api-key="apiKey"
          :marker="geocodedLocation"
          :center="geocodedLocation || { lat: 37.7749, lng: -122.4194 }"
          :address-label="
            validatedAddress?.formattedAddress ||
            [form.shipAddress, form.shipCity, form.shipPostalCode, form.shipCountry]
              .filter(Boolean)
              .join(', ')
          "
        />
        <q-banner v-else dense class="bg-grey-2 text-grey-8 q-mt-sm">
          Google Maps API key not configured. Set VITE_GOOGLE_MAPS_API_KEY.
        </q-banner>
      </div>
    </q-card-section>

    <q-separator class="q-my-md" />

    <q-card-section>
      <div class="row items-center q-mb-sm">
        <div class="text-subtitle1 section-title">Products</div>
        <q-space />
        <q-btn flat icon="add" label="Add item" @click="addLineItem" />
        <q-btn
          v-if="mode === 'edit'"
          color="primary"
          class="q-ml-sm"
          label="Save Items"
          :loading="savingItems"
          @click="saveLineItems"
        />
      </div>
      <div v-if="!form.orderDetails.length" class="text-grey-6">No items added yet.</div>
      <div v-for="(item, index) in form.orderDetails" :key="index" class="row q-col-gutter-md q-mb-sm">
        <div class="col-12 col-md-4">
          <q-select
            v-model="item.productId"
            :options="availableProducts"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            label="Product"
            dense
            outlined
            aria-label="Product selection"
            :rules="[requiredRule('Product')]"
          />
        </div>
        <div class="col-12 col-md-2">
          <q-input
            v-model="item.quantity"
            type="number"
            label="Qty"
            dense
            outlined
            aria-label="Quantity"
            :rules="[quantityRule]"
          />
        </div>
        <div class="col-12 col-md-2">
          <q-input
            v-model="item.unitPrice"
            type="number"
            label="Unit"
            dense
            outlined
            aria-label="Unit price"
            :rules="[nonNegativeRule('Unit price')]"
          />
        </div>
        <div class="col-12 col-md-2">
          <q-input
            v-model="item.discount"
            type="number"
            label="Disc"
            dense
            outlined
            aria-label="Discount"
            :rules="[nonNegativeRule('Discount'), maxDiscountRule]"
          />
        </div>
        <div class="col-12 col-md-2 flex items-center">
          <q-btn flat icon="delete" color="negative" @click="removeLineItem(index)" aria-label="Remove item" />
        </div>
      </div>
    </q-card-section>
    </q-form>
  </q-card>
</template>
