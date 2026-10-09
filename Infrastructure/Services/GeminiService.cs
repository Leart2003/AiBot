using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class GeminiService
{
    private const string Model = "gemini-2.5-flash";

    private const string SystemPrompt =
        "You are a study assistant for the university course Grid Computing and Cloud Computing. " +
        "Answer clearly and briefly. Reply in the same language as the question. " +
        "If you are not sure, say so.";

    private readonly HttpClient _http;

    public GeminiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> AskAsync(string question)
    {
        var body = new
        {
            systemInstruction = new { parts = new[] { new { text = SystemPrompt } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = question } } }
            }
        };

        try
        {
            using var response = await _http.PostAsJsonAsync($"v1beta/models/{Model}:generateContent", body);

            if (!response.IsSuccessStatusCode)
            {
                return $"Gemini returned an error ({(int)response.StatusCode}).";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates)
                || candidates.GetArrayLength() == 0
                || !candidates[0].TryGetProperty("content", out var content))
            {
                return "Gemini did not return an answer.";
            }

            var answer = new StringBuilder();
            foreach (var part in content.GetProperty("parts").EnumerateArray())
            {
                if (part.TryGetProperty("text", out var text))
                {
                    answer.Append(text.GetString());
                }
            }

            return answer.Length > 0 ? answer.ToString() : "Gemini did not return an answer.";
        }
        catch (Exception)
        {
            return "Could not get an answer from Gemini. Please try again.";
        }
    }
}