using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiBot.Controllers;

public class ChatController : Controller
{
    private readonly ChatAiService _chatAi;

    public ChatController(ChatAiService chatAi)
    {
        _chatAi = chatAi;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 1000)
        {
            return BadRequest("Please write a question (max 1000 characters).");
        }

        var answer = await _chatAi.AskAsync(request.Question);
        return Json(new { answer });
    }
}

public record AskRequest(string Question);