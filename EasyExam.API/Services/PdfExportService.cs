using EasyExam.API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EasyExam.API.Services;

public class PdfExportService
{
    public byte[] GenerateExamPdf(Exam exam)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            // Sınav sayfası
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text(exam.Title).FontSize(20).Bold().AlignCenter();
                    col.Item().Text($"Subject: {exam.Subject}  |  Grade: {exam.Grade}  |  Duration: {exam.DurationMinutes} min")
                        .FontSize(10).AlignCenter();
                    col.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    int number = 1;
                    foreach (var q in exam.Questions.OrderBy(x => x.Order))
                    {
                        col.Item().PaddingBottom(10).Column(qCol =>
                        {
                            qCol.Item().Text($"{number}. {q.Text}").Bold();

                            if (q.Type == QuestionType.MultipleChoice)
                            {
                                foreach (var option in q.Options)
                                    qCol.Item().PaddingLeft(15).Text(option);
                            }
                            else if (q.Type == QuestionType.TrueFalse)
                            {
                                qCol.Item().PaddingLeft(15).Text("A) True        B) False");
                            }
                            else if (q.Type == QuestionType.FillInTheBlank)
                            {
                                qCol.Item().PaddingLeft(15).Text("Answer: ___________________________");
                            }
                        });

                        number++;
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });

            // Cevap Anahtarı sayfası
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Answer Key").FontSize(20).Bold().AlignCenter();
                    col.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    int number = 1;
                    foreach (var q in exam.Questions.OrderBy(x => x.Order))
                    {
                        col.Item().Text($"{number}. {q.CorrectAnswer}");
                        number++;
                    }
                });
            });

        }).GeneratePdf();
    }
}