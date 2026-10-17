using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Tests;

public class AddCourseTests
{
    private static Course Draft(string title = "Git Basics") => new()
    {
        Title = title,
        ShortDescription = "Track changes with Git.",
        Category = "Programming",
        Instructor = "Ana Ruiz",
        Level = "beginner",
        Lessons =
        {
            new Lesson { Title = "What is Git", DurationMinutes = 8, Content = "Git saves versions." },
            new Lesson { Title = "Commits", DurationMinutes = 10, Content = "A commit is a snapshot." }
        }
    };

    [Fact]
    public void AddCourse_Works_AndAssignsNewUniqueIds()
    {
        var store = new InMemoryStore();
        var oldMaxCourse = store.Courses.Max(c => c.Id);
        var oldMaxLesson = store.Courses.SelectMany(c => c.Lessons).Max(l => l.Id);

        var (course, error) = store.AddCourse(Draft());

        Assert.Null(error);
        Assert.Equal(oldMaxCourse + 1, course!.Id);
        Assert.Equal("Beginner", course.Level);
        Assert.Equal(new[] { oldMaxLesson + 1, oldMaxLesson + 2 }, course.Lessons.Select(l => l.Id));
        Assert.Contains(store.SearchCourses("git"), c => c.Id == course.Id);
    }

    [Theory]
    [InlineData("title")]
    [InlineData("shortDescription")]
    [InlineData("category")]
    [InlineData("instructor")]
    public void AddCourse_RequiresTextFields(string field)
    {
        var store = new InMemoryStore();
        var draft = Draft();
        if (field == "title") draft.Title = " ";
        if (field == "shortDescription") draft.ShortDescription = "";
        if (field == "category") draft.Category = "";
        if (field == "instructor") draft.Instructor = "";

        var (course, error) = store.AddCourse(draft);

        Assert.Null(course);
        Assert.Equal($"{field} is required.", error);
    }

    [Fact]
    public void AddCourse_RejectsBadLevel()
    {
        var store = new InMemoryStore();
        var draft = Draft();
        draft.Level = "Expert";

        Assert.Equal("level must be Beginner, Intermediate or Advanced.", store.AddCourse(draft).Error);
    }

    [Fact]
    public void AddCourse_RejectsMissingOrBadLessons()
    {
        var store = new InMemoryStore();

        var none = Draft();
        none.Lessons.Clear();
        Assert.Equal("At least one lesson is required.", store.AddCourse(none).Error);

        var noTitle = Draft();
        noTitle.Lessons[0].Title = "";
        Assert.Equal("Every lesson needs a title.", store.AddCourse(noTitle).Error);

        var noTime = Draft();
        noTime.Lessons[0].DurationMinutes = 0;
        Assert.Equal("Every lesson needs durationMinutes above 0.", store.AddCourse(noTime).Error);
    }

    [Fact]
    public void AddCourse_RejectsDuplicateTitleIgnoringCase()
    {
        var store = new InMemoryStore();
        Assert.Null(store.AddCourse(Draft("Git Basics")).Error);

        Assert.Equal("A course with this title already exists.", store.AddCourse(Draft("GIT BASICS")).Error);
    }
}
