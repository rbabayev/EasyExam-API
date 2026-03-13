using EasyExam.API.Models;

namespace EasyExam.API.DTOs
{
    public class CreateQuestionDto
    {
        public string Text { get; set; } = "";
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = "";
        public int Order { get; set; }
    }
}
