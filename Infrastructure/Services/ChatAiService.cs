using System.Net.Http.Json;
using System.Text.Json;
using Domain.Entities;

namespace Infrastructure.Services;

public class ChatAiService
{


    private const string Model = "openai/gpt-oss-120b";


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

  
    public async Task<string?> AskAsync(string question, IEnumerable<ChatMessage> history)
    {
        var messages = new List<object>
        {
            new { role = "system", content = SystemPrompt }
        };

        foreach (var message in history)
        {
            messages.Add(new { role = message.Role, content = message.Text });
        }

        messages.Add(new { role = "user", content = question });

        var body = new { model = Model, messages };

        try
        {
            using var response = await _http.PostAsJsonAsync("chat/completions", body);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"AI service error {(int)response.StatusCode}: {errorBody}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return string.IsNullOrWhiteSpace(answer)
                ? null
                : answer.Replace("**", "").Replace("###", "");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI service exception: {ex.Message}");
            return null;
        }
    }
}

