using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record CartRequest(string? StudentName, int CourseId);

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        app.MapGet("/api/cart", (string? studentName, InMemoryStore store) =>
            store.GetCart(studentName).Select(i =>
            {
                var course = store.Courses.First(c => c.Id == i.CourseId);
                return new { i.CourseId, course.Title, course.Price, course.Category, course.Instructor };
            }));

        app.MapPost("/api/cart", (CartRequest request, InMemoryStore store) =>
        {
            var (item, error) = store.AddToCart(request.StudentName, request.CourseId);
            if (item is null)
                return error is "You already own this course." or "Already in your cart."
                    ? Results.Conflict(new { error })
                    : Results.BadRequest(new { error });

            return Results.Created($"/api/cart?studentName={Uri.EscapeDataString(item.StudentName)}", item);
        });

        app.MapDelete("/api/cart", (string? studentName, int courseId, InMemoryStore store) =>
            store.RemoveFromCart(studentName, courseId) ? Results.NoContent() : Results.NotFound());
    }
}
