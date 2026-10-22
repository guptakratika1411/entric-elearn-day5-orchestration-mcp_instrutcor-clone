import { Routes, Route } from 'react-router-dom'
import NavBar from './components/NavBar.jsx'
import CoursesPage from './pages/CoursesPage.jsx'
import CourseDetailPage from './pages/CourseDetailPage.jsx'
import MyCoursesPage from './pages/MyCoursesPage.jsx'
import QuizPage from './pages/QuizPage.jsx'
import ProgressPage from './pages/ProgressPage.jsx'
import BookmarksPage from './pages/BookmarksPage.jsx'
import CertificatesPage from './pages/CertificatesPage.jsx'
import InstructorPage from './pages/InstructorPage.jsx'
import CartPage from './pages/CartPage.jsx'
import MyOrdersPage from './pages/MyOrdersPage.jsx'
import InstructorRevenuePage from './pages/InstructorRevenuePage.jsx'

export default function App() {
  return (
    <>
      <NavBar />
      <main className="container">
        <Routes>
          <Route path="/" element={<CoursesPage />} />
          <Route path="/courses/:id" element={<CourseDetailPage />} />
          <Route path="/my-courses" element={<MyCoursesPage />} />
          <Route path="/lessons/:lessonId/quiz" element={<QuizPage />} />
          <Route path="/progress" element={<ProgressPage />} />
          <Route path="/bookmarks" element={<BookmarksPage />} />
          <Route path="/certificates" element={<CertificatesPage />} />
          <Route path="/instructor" element={<InstructorPage />} />
          <Route path="/cart" element={<CartPage />} />
          <Route path="/orders" element={<MyOrdersPage />} />
          <Route path="/instructor-revenue" element={<InstructorRevenuePage />} />
        </Routes>
      </main>
    </>
  )
}
