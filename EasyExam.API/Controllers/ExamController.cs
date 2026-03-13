using EasyExam.API.Data;
using EasyExam.API.DTOs;
using EasyExam.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExamController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/exam
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _context.Exams
                .Include(e => e.Questions)
                .ToListAsync();
            return Ok(exams);
        }

        // GET: api/exam/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam is null) return NotFound();
            return Ok(exam);
        }

        // POST: api/exam
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExamDto dto)
        {
            var exam = new Exam
            {
                Title = dto.Title,
                Subject = dto.Subject,
                Grade = dto.Grade,
                DurationMinutes = dto.DurationMinutes
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = exam.Id }, exam);
        }

        // PUT: api/exam/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExamDto dto)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam is null) return NotFound();

            exam.Title = dto.Title;
            exam.Subject = dto.Subject;
            exam.Grade = dto.Grade;
            exam.DurationMinutes = dto.DurationMinutes;

            await _context.SaveChangesAsync();
            return Ok(exam);
        }

        // DELETE: api/exam/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam is null) return NotFound();

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
