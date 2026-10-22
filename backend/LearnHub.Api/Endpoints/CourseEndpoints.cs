using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Api.Endpoints;

public record LessonRequest(string? Title, int DurationMinutes, string? Content);

public record CourseRequest(
    string? Title, string? ShortDescription, string? Category,
    string? Instructor, string? Level, List<LessonRequest>? Lessons, decimal Price = 0);

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        app.MapGet("/api/courses", (string? search, InMemoryStore store) =>
            store.SearchCourses(search)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.ShortDescription,
                c.Category,
                c.Instructor,
                c.Level,
                c.Price,
                LessonCount = c.Lessons.Count
            }));

        app.MapPost("/api/courses", (CourseRequest request, InMemoryStore store) =>
        {
            var draft = new Course
            {
                Title = request.Title ?? "",
                ShortDescription = request.ShortDescription ?? "",
                Category = request.Category ?? "",
                Instructor = request.Instructor ?? "",
                Level = request.Level ?? "",
                Price = request.Price,
                Lessons = (request.Lessons ?? new()).Select(l => new Lesson
                {
                    Title = l.Title ?? "",
                    DurationMinutes = l.DurationMinutes,
                    Content = l.Content ?? ""
                }).ToList()
            };
            var (course, error) = store.AddCourse(draft);
            if (course is null)
                return error == "A course with this title already exists."
                    ? Results.Conflict(new { error })
                    : Results.BadRequest(new { error });

            return Results.Created($"/api/courses/{course.Id}", new
            {
                course.Id,
                course.Title,
                course.ShortDescription,
                course.Category,
                course.Instructor,
                course.Level,
                LessonCount = course.Lessons.Count
            });
        });

        app.MapGet("/api/courses/{id:int}", (int id, InMemoryStore store) =>
        {
            var course = store.Courses.FirstOrDefault(c => c.Id == id);
            if (course is null) return Results.NotFound();

            return Results.Ok(new
            {
                course.Id,
                course.Title,
                course.ShortDescription,
                course.Category,
                course.Instructor,
                course.Level,
                course.Price,
                Lessons = course.Lessons.Select(l => new
                {
                    l.Id,
                    l.Title,
                    l.DurationMinutes,
                    l.Content
                })
            });
        });
    }
}
