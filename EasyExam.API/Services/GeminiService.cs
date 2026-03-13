using EasyExam.API.DTOs;
using EasyExam.API.Models;
using System.Text;
using System.Text.Json;

namespace EasyExam.API.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"]!;
        _model = config["Gemini:Model"]!;
    }

    public async Task<List<Question>> GenerateQuestionsAsync(GeminiRequestDto dto)
    {
        var prompt = BuildPrompt(dto);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return ParseResponse(responseJson);
    }

    private string BuildPrompt(GeminiRequestDto dto)
    {
        var typeText = dto.QuestionType switch
        {
            "MultipleChoice" => "multiple choice (with options A, B, C, D)",
            "TrueFalse" => "true/false",
            "FillInTheBlank" => "fill in the blank",
            _ => "multiple choice"
        };

        return $"You are a professional teacher. Generate questions based on the following criteria:\n\n" +
           $"- Subject: {dto.Subject}\n" +
           $"- Grade: {dto.Grade}\n" +
           $"- Topic: {dto.Topic}\n" +
           $"- Number of questions: {dto.QuestionCount}\n" +
           $"- Question type: {typeText}\n\n" +
           "Return ONLY a JSON array, no extra text, no markdown.\n" +
           "Format:\n" +
           "[\n" +
           "  {\n" +
           "    \"text\": \"Question text here\",\n" +
           $"    \"type\": \"{dto.QuestionType}\",\n" +
           "    \"options\": [\"A) ...\", \"B) ...\", \"C) ...\", \"D) ...\"],\n" +
           "    \"correctAnswer\": \"A\",\n" +
           "    \"order\": 1\n" +
           "  }\n" +
           "]\n\n" +
           "For True/False questions options must be: [\"True\", \"False\"].\n" +
           "For Fill in the Blank questions options must be an empty array.";
    }
    private List<Question> ParseResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);

        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";

        // JSON bloğunu temizle
        text = text.Replace("```json", "").Replace("```", "").Trim();

        var questions = JsonSerializer.Deserialize<List<AiQuestion>>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        return questions.Select(q => new Question
        {
            Text = q.Text,
            Type = Enum.TryParse<QuestionType>(q.Type, out var t) ? t : QuestionType.MultipleChoice,
            Options = q.Options,
            CorrectAnswer = q.CorrectAnswer,
            Order = q.Order
        }).ToList();
    }
}

// Gemini'den gelen JSON'u parse etmek için
internal class AiQuestion
{
    public string Text { get; set; } = "";
    public string Type { get; set; } = "";
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = "";
    public int Order { get; set; }
}