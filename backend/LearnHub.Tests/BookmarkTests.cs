using LearnHub.Api.Data;

namespace LearnHub.Tests;

public class BookmarkTests
{
    [Fact]
    public void AddBookmark_Works_ThenRejectsDuplicateIgnoringCase()
    {
        var store = new InMemoryStore();
        var first = store.AddBookmark("Asha", 1);
        var second = store.AddBookmark("ASHA", 1);

        Assert.NotNull(first.Bookmark);
        Assert.Null(second.Bookmark);
        Assert.Equal("Already bookmarked.", second.Error);
    }

    [Fact]
    public void AddBookmark_RejectsEmptyNameAndUnknownCourse()
    {
        var store = new InMemoryStore();
        Assert.Equal("studentName is required.", store.AddBookmark(" ", 1).Error);
        Assert.Equal("Course not found.", store.AddBookmark("Asha", 999).Error);
    }

    [Fact]
    public void GetBookmarks_ReturnsOnlyThatStudentsBookmarks()
    {
        var store = new InMemoryStore();
        store.AddBookmark("Asha", 1);
        store.AddBookmark("Asha", 2);
        store.AddBookmark("Ben", 3);

        Assert.Equal(new[] { 1, 2 }, store.GetBookmarks("asha").Select(b => b.CourseId));
        Assert.Single(store.GetBookmarks("Ben"));
        Assert.Empty(store.GetBookmarks("Nobody"));
    }

    [Fact]
    public void RemoveBookmark_RemovesOnce_ThenReturnsFalse()
    {
        var store = new InMemoryStore();
        store.AddBookmark("Asha", 1);

        Assert.True(store.RemoveBookmark("ASHA", 1));
        Assert.False(store.RemoveBookmark("Asha", 1));
        Assert.Empty(store.GetBookmarks("Asha"));
    }

    [Fact]
    public void BookmarkIds_AreNeverReused()
    {
        var store = new InMemoryStore();
        var a = store.AddBookmark("Asha", 1).Bookmark!;
        store.RemoveBookmark("Asha", 1);
        var b = store.AddBookmark("Asha", 1).Bookmark!;

        Assert.NotEqual(a.Id, b.Id);
    }
}
