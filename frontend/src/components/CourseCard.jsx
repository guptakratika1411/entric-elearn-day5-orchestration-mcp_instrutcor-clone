import { Link } from 'react-router-dom'

// onToggleBookmark is optional. When given, a bookmark button is shown.
export default function CourseCard({ course, bookmarked = false, onToggleBookmark }) {
  function handleBookmark(e) {
    e.preventDefault() // the card is a link; do not open the course
    e.stopPropagation()
    onToggleBookmark(course.id, bookmarked)
  }

  return (
    <Link to={`/courses/${course.id}`} className="card">
      <div className="card-top">
        <span className="tag">{course.category}</span>
        {onToggleBookmark && (
          <button
            type="button"
            className={`bookmark-btn${bookmarked ? ' on' : ''}`}
            onClick={handleBookmark}
            aria-pressed={bookmarked}
            aria-label={bookmarked ? 'Remove bookmark' : 'Add bookmark'}
            title={bookmarked ? 'Remove bookmark' : 'Bookmark this course'}
          >
            {bookmarked ? '★' : '☆'}
          </button>
        )}
      </div>
      <h3>{course.title}</h3>
      <p>{course.shortDescription}</p>
      <div className="meta">
        <span>{course.instructor}</span>
        <span>{course.level} · {course.lessonCount} lessons</span>
      </div>
    </Link>
  )
}
