using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class GeminiService
{
    private const string Model = "gemini-flash-lite-latest";

    private const string SystemPrompt =
       "You are a study assistant for the university course Grid Computing and Cloud Computing. " +
       "Answer clearly and briefly. Reply in the same language as the question. " +
       "If you are not sure, say so. " +
       "Write in plain text only. Do not use Markdown: no asterisks, no # headings, no bold. " +
       "For lists, put each item on its own line starting with a dash (-).";

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
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"Gemini returned an error ({(int)response.StatusCode}): {errorBody}";
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