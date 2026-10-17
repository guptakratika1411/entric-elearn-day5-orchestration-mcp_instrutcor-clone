using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record BookmarkRequest(string? StudentName, int CourseId);

public static class BookmarkEndpoints
{
    public static void MapBookmarkEndpoints(this WebApplication app)
    {
        app.MapPost("/api/bookmarks", (BookmarkRequest request, InMemoryStore store) =>
        {
            var (bookmark, error) = store.AddBookmark(request.StudentName, request.CourseId);
            if (bookmark is null)
                return error == "Already bookmarked."
                    ? Results.Conflict(new { error })
                    : Results.BadRequest(new { error });

            return Results.Created($"/api/bookmarks/{bookmark.Id}", bookmark);
        });

        app.MapDelete("/api/bookmarks", (string? studentName, int courseId, InMemoryStore store) =>
            store.RemoveBookmark(studentName, courseId) ? Results.NoContent() : Results.NotFound());

        app.MapGet("/api/bookmarks", (string? studentName, InMemoryStore store) =>
            store.GetBookmarks(studentName).Select(b =>
            {
                var course = store.Courses.First(c => c.Id == b.CourseId);
                return new
                {
                    b.Id,
                    b.CourseId,
                    CourseTitle = course.Title,
                    course.ShortDescription,
                    course.Category,
                    course.Instructor,
                    course.Level,
                    LessonCount = course.Lessons.Count,
                    b.StudentName
                };
            }));
    }
}
