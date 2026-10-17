namespace LearnHub.Api.Models;

public class Bookmark
{
    public int Id { get; set; }
    public string StudentName { get; set; } = "";
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
