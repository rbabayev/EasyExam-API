namespace EasyExam.API.Models;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Grade { get; set; } = "";
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Question> Questions { get; set; } = new();
}