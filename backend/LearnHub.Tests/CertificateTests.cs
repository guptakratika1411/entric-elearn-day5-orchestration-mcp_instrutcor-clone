using LearnHub.Api.Data;

namespace LearnHub.Tests;

public class CertificateTests
{
    private static void CompleteAll(InMemoryStore store, string name, int courseId)
    {
        foreach (var lesson in store.Courses.First(c => c.Id == courseId).Lessons)
            store.MarkLessonComplete(name, courseId, lesson.Id);
    }

    [Fact]
    public void NoCertificate_WhenCourseIsNotFinished()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");
        store.MarkLessonComplete("Asha", 1, 1);

        Assert.Empty(store.GetCertificates("Asha"));
    }

    [Fact]
    public void Certificate_IsIssued_WhenAllLessonsAreComplete()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");
        CompleteAll(store, "Asha", 1);

        var cert = Assert.Single(store.GetCertificates("asha"));

        Assert.Equal("Intro to C# Programming", cert.CourseTitle);
        Assert.Equal("Asha", cert.StudentName);
        Assert.False(string.IsNullOrWhiteSpace(cert.CertificateId));
        Assert.True(cert.IssuedOn <= DateTime.UtcNow);
    }

    [Fact]
    public void Certificates_BelongToTheRightStudent()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");
        store.Enroll(1, "Ben");
        CompleteAll(store, "Asha", 1);

        Assert.Single(store.GetCertificates("Asha"));
        Assert.Empty(store.GetCertificates("Ben"));
        Assert.Empty(store.GetCertificates(""));
    }

    [Fact]
    public void CertificateIds_AreUniquePerCourse_AndStable()
    {
        var store = new InMemoryStore();
        store.Enroll(1, "Asha");
        store.Enroll(2, "Asha");
        CompleteAll(store, "Asha", 1);
        CompleteAll(store, "Asha", 2);

        var first = store.GetCertificates("Asha");
        var second = store.GetCertificates("Asha");

        Assert.Equal(2, first.Select(c => c.CertificateId).Distinct().Count());
        Assert.Equal(first.Select(c => c.CertificateId), second.Select(c => c.CertificateId));
    }
}
