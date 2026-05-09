export const buildOrderReportData = ({ meta, items }) => ({
  header: {
    title: 'Northwind Traders',
    subtitle: 'Order Report',
  },
  meta: {
    orderId: meta.orderId,
    orderDate: meta.orderDate,
    customer: meta.customer,
    freight: meta.freight,
    total: meta.total,
  },
  items: items.map((item) => ({
    productId: item.productId,
    productName: item.productName,
    category: item.category,
    quantity: item.quantity,
    unitPrice: item.unitPrice,
    discount: item.discount,
    total: item.total,
  })),
})
