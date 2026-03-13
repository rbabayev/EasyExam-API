using System.Text.Json.Serialization;

namespace EasyExam.API.Models;

public class Question
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string Text { get; set; } = "";
    public QuestionType Type { get; set; }
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = "";
    public int Order { get; set; }

    [JsonIgnore]
    public Exam Exam { get; set; } = null!;
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    FillInTheBlank
}