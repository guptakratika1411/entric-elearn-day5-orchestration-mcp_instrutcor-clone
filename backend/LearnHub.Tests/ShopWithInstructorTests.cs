using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Tests;

// The shop and the Day 5 features must work together.
public class ShopWithInstructorTests
{
    private static Course Draft(decimal price = 0) => new()
    {
        Title = "Git Basics",
        ShortDescription = "Track changes with Git.",
        Category = "Programming",
        Instructor = "Ana Ruiz",
        Level = "Beginner",
        Price = price,
        Lessons = { new Lesson { Title = "Commits", DurationMinutes = 10 } }
    };

    [Fact]
    public void InstructorCourse_IsFreeByDefault_AndCanHaveAPrice()
    {
        var store = new InMemoryStore();
        Assert.Equal(0m, store.AddCourse(Draft()).Course!.Price);

        var paid = store.AddCourse(new Course
        {
            Title = "Paid Git", ShortDescription = "x", Category = "Programming", Instructor = "Ana Ruiz",
            Level = "Beginner", Price = 25m, Lessons = { new Lesson { Title = "One", DurationMinutes = 5 } }
        }).Course!;
        Assert.Equal(25m, paid.Price);
        Assert.NotNull(store.AddToCart("Asha", paid.Id).Item);
    }

    [Fact]
    public void InstructorCourse_RejectsNegativePrice()
    {
        var store = new InMemoryStore();
        Assert.Equal("price cannot be below 0.", store.AddCourse(Draft(-1)).Error);
    }

    [Fact]
    public void Refund_KeepsBookmarks_AndRemovesTheCertificateRoute()
    {
        var store = new InMemoryStore();
        store.AddBookmark("Asha", 2);
        store.AddToCart("Asha", 2);
        var orderId = store.Checkout("Asha", null).Order!.Id;
        store.RefundOrder(orderId);

        Assert.Single(store.GetBookmarks("Asha"));
        Assert.Empty(store.GetCertificates("Asha"));
        Assert.False(store.IsEnrolled("Asha", 2));
    }

    [Fact]
    public void FinishedCourse_CannotBeRefunded_SoItsCertificateStays()
    {
        var store = new InMemoryStore();
        store.AddToCart("Asha", 4);
        var orderId = store.Checkout("Asha", null).Order!.Id;
        foreach (var lesson in store.Courses.First(c => c.Id == 4).Lessons)
            store.MarkLessonComplete("Asha", 4, lesson.Id);

        Assert.NotNull(store.RefundOrder(orderId).Error);
        Assert.Single(store.GetCertificates("Asha"));
    }
}
