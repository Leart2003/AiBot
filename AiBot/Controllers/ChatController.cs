using System.Security.Claims;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiBot.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly ChatAiService _chatAi;
    private readonly ApplicationDbContext _db;

    public ChatController(ChatAiService chatAi, ApplicationDbContext db)
    {
        _chatAi = chatAi;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var messages = await _db.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.Id)
            .ToListAsync();

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 1000)
        {
            return BadRequest("Please write a question (max 1000 characters).");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // The last 6 messages give the bot some memory of the conversation.
        var recent = await _db.ChatMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Id)
            .Take(6)
            .ToListAsync();
        recent.Reverse();

        var answer = await _chatAi.AskAsync(request.Question, recent);

        if (answer == null)
        {
            return Json(new { answer = "Sorry, I could not get an answer right now. Please try again." });
        }

        _db.ChatMessages.Add(new ChatMessage
        {
            UserId = userId,
            Role = "user",
            Text = request.Question,
            CreatedAtUtc = DateTime.UtcNow
        });
        _db.ChatMessages.Add(new ChatMessage
        {
            UserId = userId,
            Role = "assistant",
            Text = answer,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return Json(new { answer });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _db.ChatMessages
            .Where(m => m.UserId == userId)
            .ExecuteDeleteAsync();

        return RedirectToAction(nameof(Index));
    }
}

public record AskRequest(string Question);