using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using EasyExam.API.Models;

namespace EasyExam.API.Services;

public class WordExportService
{
    public byte[] GenerateExamWord(Exam exam)
    {
        using var memoryStream = new MemoryStream();
        using var doc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);

        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body!;

        // Başlık
        body.AppendChild(CreateParagraph(exam.Title, "36", true, JustificationValues.Center));
        body.AppendChild(CreateParagraph($"Subject: {exam.Subject} | Grade: {exam.Grade} | Duration: {exam.DurationMinutes} min", "20", false, JustificationValues.Center));
        body.AppendChild(CreateParagraph("", "20", false, JustificationValues.Left));

        // Sorular
        int number = 1;
        foreach (var q in exam.Questions.OrderBy(x => x.Order))
        {
            body.AppendChild(CreateParagraph($"{number}. {q.Text}", "22", true, JustificationValues.Left));

            if (q.Type == QuestionType.MultipleChoice)
            {
                foreach (var option in q.Options)
                    body.AppendChild(CreateParagraph($"    {option}", "22", false, JustificationValues.Left));
            }
            else if (q.Type == QuestionType.TrueFalse)
            {
                body.AppendChild(CreateParagraph("    A) True        B) False", "22", false, JustificationValues.Left));
            }
            else if (q.Type == QuestionType.FillInTheBlank)
            {
                body.AppendChild(CreateParagraph("    Answer: ___________________________", "22", false, JustificationValues.Left));
            }

            body.AppendChild(CreateParagraph("", "20", false, JustificationValues.Left));
            number++;
        }

        // Cevap Anahtarı — yeni sayfa
        body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        body.AppendChild(CreateParagraph("Answer Key", "32", true, JustificationValues.Center));
        body.AppendChild(CreateParagraph("", "20", false, JustificationValues.Left));

        int answerNumber = 1;
        foreach (var q in exam.Questions.OrderBy(x => x.Order))
        {
            body.AppendChild(CreateParagraph($"{answerNumber}. {q.CorrectAnswer}", "22", false, JustificationValues.Left));
            answerNumber++;
        }

        mainPart.Document.Save();
        doc.Dispose();

        return memoryStream.ToArray();
    }

    private Paragraph CreateParagraph(string text, string fontSize, bool bold, JustificationValues justify)
    {
        return new Paragraph(
            new ParagraphProperties(new Justification { Val = justify }),
            new Run(
                new RunProperties(
                    new FontSize { Val = fontSize },
                    bold ? new Bold() : new Bold { Val = false }
                ),
                new Text(text) { Space = SpaceProcessingModeValues.Preserve }
            )
        );
    }
}