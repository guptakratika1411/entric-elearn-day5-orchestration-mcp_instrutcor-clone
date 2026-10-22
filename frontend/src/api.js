// All calls to the backend live here.

async function request(url, options) {
  const res = await fetch(url, options)
  const body = await res.json().catch(() => null)
  if (!res.ok) {
    throw new Error(body?.error || `Request failed (${res.status})`)
  }
  return body
}

export const getCourses = (search = '') =>
  request(`/api/courses?search=${encodeURIComponent(search)}`)

export const getCourse = (id) => request(`/api/courses/${id}`)

export const enroll = (courseId, studentName) =>
  request('/api/enrollments', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ courseId, studentName }),
  })

export const getEnrollments = (studentName) =>
  request(`/api/enrollments?studentName=${encodeURIComponent(studentName)}`)

// The student name is remembered in the browser.
export const getSavedName = () => localStorage.getItem('studentName') || ''
export const saveName = (name) => localStorage.setItem('studentName', name)

export const getQuiz = (lessonId) => request(`/api/lessons/${lessonId}/quiz`)

export const submitQuiz = (lessonId, studentName, answers) =>
  request(`/api/lessons/${lessonId}/quiz/submit`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, answers }),
  })

export const markLessonComplete = (studentName, courseId, lessonId) =>
  request('/api/progress', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, courseId, lessonId }),
  })

export const getProgress = (studentName) =>
  request(`/api/progress?studentName=${encodeURIComponent(studentName)}`)

export const getBookmarks = (studentName) =>
  request(`/api/bookmarks?studentName=${encodeURIComponent(studentName)}`)

export const addBookmark = (studentName, courseId) =>
  request('/api/bookmarks', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, courseId }),
  })

export const removeBookmark = (studentName, courseId) =>
  request(`/api/bookmarks?studentName=${encodeURIComponent(studentName)}&courseId=${courseId}`, {
    method: 'DELETE',
  })

export const getCertificates = (studentName) =>
  request(`/api/certificates?studentName=${encodeURIComponent(studentName)}`)

export const createCourse = (course) =>
  request('/api/courses', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(course),
  })

export const formatPrice = (price) => (price > 0 ? `$${Number(price).toFixed(2)}` : 'Free')

export const getCart = (studentName) =>
  request(`/api/cart?studentName=${encodeURIComponent(studentName)}`)

export const addToCart = (studentName, courseId) =>
  request('/api/cart', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, courseId }),
  })

export const removeFromCart = (studentName, courseId) =>
  request(`/api/cart?studentName=${encodeURIComponent(studentName)}&courseId=${courseId}`, {
    method: 'DELETE',
  })

export const previewCheckout = (studentName, couponCode) =>
  request('/api/checkout/preview', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, couponCode }),
  })

export const checkout = (studentName, couponCode) =>
  request('/api/checkout', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, couponCode }),
  })

export const getOrders = (studentName) =>
  request(`/api/orders?studentName=${encodeURIComponent(studentName)}`)

export const refundOrder = (orderId) => request(`/api/orders/${orderId}/refund`, { method: 'POST' })

export const getInstructorRevenue = () => request('/api/instructors/revenue')
