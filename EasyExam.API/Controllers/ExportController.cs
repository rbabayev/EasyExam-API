using EasyExam.API.Data;
using EasyExam.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly WordExportService _wordService;
        private readonly PdfExportService _pdfService;

        public ExportController(AppDbContext context, WordExportService wordService, PdfExportService pdfService)
        {
            _context = context;
            _wordService = wordService;
            _pdfService = pdfService;
        }

        // GET: api/export/word/5
        [HttpGet("word/{examId}")]
        public async Task<IActionResult> ExportWord(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam is null) return NotFound();

            var bytes = _wordService.GenerateExamWord(exam);
            var fileName = $"{exam.Title.Replace(" ", "_")}_Exam.docx";

            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }

        // GET: api/export/pdf/5
        [HttpGet("pdf/{examId}")]
        public async Task<IActionResult> ExportPdf(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam is null) return NotFound();

            var bytes = _pdfService.GenerateExamPdf(exam);
            var fileName = $"{exam.Title.Replace(" ", "_")}_Exam.pdf";

            return File(bytes, "application/pdf", fileName);
        }
    }
}
