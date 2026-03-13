using EasyExam.API.Data;
using EasyExam.API.DTOs;
using EasyExam.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly AppDbContext _context;

        public AiController(GeminiService geminiService, AppDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        // POST: api/ai/generate/{examId}
        [HttpPost("generate/{examId}")]
        public async Task<IActionResult> Generate(int examId, [FromBody] GeminiRequestDto dto)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam is null) return NotFound("Sınav bulunamadı.");

            var questions = await _geminiService.GenerateQuestionsAsync(dto);

            foreach (var q in questions)
                q.ExamId = examId;

            _context.Questions.AddRange(questions);
            await _context.SaveChangesAsync();

            return Ok(questions);
        }
    }
}
