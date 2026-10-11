using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

public class ChatAiService
{
    // Change this to a model name from your provider's model list.
    private const string Model = "llama-3.3-70b-versatile";

    private const string SystemPrompt =
        "You are a study assistant for the university course Grid Computing and Cloud Computing. " +
        "Answer clearly and briefly. Reply in the same language as the question. " +
        "If you are not sure, say so. " +
        "Write in plain text only. Do not use Markdown: no asterisks, no # headings, no bold. " +
        "For lists, put each item on its own line starting with a dash (-).";

    private readonly HttpClient _http;

    public ChatAiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> AskAsync(string question)
    {
        var body = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = question }
            }
        };

        try
        {
            using var response = await _http.PostAsJsonAsync("chat/completions", body);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"The AI service returned an error ({(int)response.StatusCode}): {errorBody}";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return string.IsNullOrWhiteSpace(answer)
                ? "The AI service did not return an answer."
                : answer.Replace("**", "").Replace("###", "");
        }
        catch (Exception)
        {
            return "Could not get an answer. Please try again.";
        }
    }
}