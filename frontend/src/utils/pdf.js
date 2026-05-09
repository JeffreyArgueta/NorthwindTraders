import { jsPDF } from 'jspdf'
import autoTable from 'jspdf-autotable'

export const exportOrderReportPdf = (filename, report) => {
  const doc = new jsPDF({ unit: 'pt', format: 'a4' })
  const marginX = 40
  let cursorY = 50

  doc.setFont('helvetica', 'bold')
  doc.setFontSize(16)
  doc.text(report.header.title, marginX, cursorY)
  doc.setFontSize(12)
  doc.setFont('helvetica', 'normal')
  doc.text(report.header.subtitle, marginX, cursorY + 18)
  cursorY += 40

  doc.setFontSize(10)
  doc.setTextColor('#4b5563')
  doc.text(`Order #: ${report.meta.orderId}`, marginX, cursorY)
  doc.text(`Order Date: ${report.meta.orderDate}`, marginX, cursorY + 14)
  doc.text(`Customer: ${report.meta.customer}`, marginX, cursorY + 28)
  doc.text(`Freight: ${Number(report.meta.freight || 0).toFixed(2)}`, marginX, cursorY + 42)
  cursorY += 60

  doc.setTextColor('#111827')

  const autoTableFn = autoTable?.default || autoTable
  if (typeof autoTableFn !== 'function') {
    throw new Error('PDF export is unavailable: autoTable not loaded')
  }

  autoTableFn(doc, {
    startY: cursorY,
    head: [['ID', 'Product', 'Category', 'Qty', 'Unit', 'Disc', 'Total']],
    body: report.items.map((item) => [
      String(item.productId),
      String(item.productName),
      String(item.category),
      String(item.quantity),
      String(item.unitPrice),
      String(item.discount),
      String(item.total),
    ]),
    theme: 'grid',
    styles: { fontSize: 9, cellPadding: 4 },
    headStyles: { fillColor: [17, 24, 39], textColor: 255 },
    margin: { left: marginX, right: marginX },
  })

  const finalY = doc.lastAutoTable?.finalY || cursorY
  doc.setFont('helvetica', 'bold')
  doc.text(`Total: ${Number(report.meta.total || 0).toFixed(2)}`, marginX, finalY + 20)

  doc.save(filename)
}
