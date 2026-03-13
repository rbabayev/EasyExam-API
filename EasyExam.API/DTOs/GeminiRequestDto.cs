namespace EasyExam.API.DTOs
{
    public class GeminiRequestDto
    {
        public string Subject { get; set; } = "";
        public string Grade { get; set; } = "";
        public string Topic { get; set; } = "";
        public int QuestionCount { get; set; } = 5;
        public string QuestionType { get; set; } = "MultipleChoice";
    }
}
