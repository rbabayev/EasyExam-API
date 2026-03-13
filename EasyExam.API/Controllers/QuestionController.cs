using EasyExam.API.Data;
using EasyExam.API.DTOs;
using EasyExam.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyExam.API.Controllers
{
    [Route("api/exam/{examId}/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/exam/5/questions
        [HttpGet]
        public async Task<IActionResult> GetAll(int examId)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam is null) return NotFound("Sınav bulunamadı.");

            var questions = await _context.Questions
                .Where(q => q.ExamId == examId)
                .OrderBy(q => q.Order)
                .ToListAsync();

            return Ok(questions);
        }

        // GET: api/exam/5/questions/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int examId, int id)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id && q.ExamId == examId);

            if (question is null) return NotFound();
            return Ok(question);
        }

        // POST: api/exam/5/questions
        [HttpPost]
        public async Task<IActionResult> Create(int examId, [FromBody] CreateQuestionDto dto)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam is null) return NotFound("Sınav bulunamadı.");

            var question = new Question
            {
                ExamId = examId,
                Text = dto.Text,
                Type = dto.Type,
                Options = dto.Options,
                CorrectAnswer = dto.CorrectAnswer,
                Order = dto.Order
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { examId, id = question.Id }, question);
        }

        // POST: api/exam/5/questions/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(int examId, [FromBody] List<CreateQuestionDto> dtos)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam is null) return NotFound("Sınav bulunamadı.");

            var questions = dtos.Select(dto => new Question
            {
                ExamId = examId,
                Text = dto.Text,
                Type = dto.Type,
                Options = dto.Options,
                CorrectAnswer = dto.CorrectAnswer,
                Order = dto.Order
            }).ToList();

            _context.Questions.AddRange(questions);
            await _context.SaveChangesAsync();

            return Ok(questions);
        }

        // PUT: api/exam/5/questions/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int examId, int id, [FromBody] CreateQuestionDto dto)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id && q.ExamId == examId);

            if (question is null) return NotFound();

            question.Text = dto.Text;
            question.Type = dto.Type;
            question.Options = dto.Options;
            question.CorrectAnswer = dto.CorrectAnswer;
            question.Order = dto.Order;

            await _context.SaveChangesAsync();
            return Ok(question);
        }

        // DELETE: api/exam/5/questions/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int examId, int id)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == id && q.ExamId == examId);

            if (question is null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/exam/5/questions/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder(int examId, [FromBody] List<ReorderDto> dtos)
        {
            var questions = await _context.Questions
                .Where(q => q.ExamId == examId)
                .ToListAsync();

            foreach (var q in questions)
            {
                var match = dtos.FirstOrDefault(d => d.Id == q.Id);
                if (match is not null) q.Order = match.Order;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
