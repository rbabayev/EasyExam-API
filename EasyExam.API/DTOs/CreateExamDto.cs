namespace EasyExam.API.DTOs
{
    public class CreateExamDto
    {
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Grade { get; set; } = "";
        public int DurationMinutes { get; set; }
    }
}
