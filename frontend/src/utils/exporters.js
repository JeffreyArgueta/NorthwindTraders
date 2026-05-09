export const exportToCsv = (filename, rows) => {
  const csvContent = rows.map((row) => row.map(escapeCsv).join(',')).join('\n')
  downloadFile(filename, `\uFEFF${csvContent}`, 'text/csv;charset=utf-8;')
}

const escapeCsv = (value) => {
  if (value === null || value === undefined) return ''
  const stringValue = String(value)
  const sanitized = sanitizeCsvFormula(stringValue)
  if (/[",\n]/.test(sanitized)) return `"${sanitized.replace(/"/g, '""')}"`
  return sanitized
}

const sanitizeCsvFormula = (value) => {
  if (/^[=+\-@]/.test(value)) return `'${value}`
  return value
}

const downloadFile = (filename, content, type) => {
  const blob = new Blob([content], { type })
  downloadBlob(filename, blob)
}

const downloadBlob = (filename, blob) => {
  const link = document.createElement('a')
  const url = URL.createObjectURL(blob)
  link.href = url
  link.setAttribute('download', filename)
  document.body.appendChild(link)
  link.click()
  link.remove()
  URL.revokeObjectURL(url)
}
