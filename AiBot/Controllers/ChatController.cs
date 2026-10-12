using System.Security.Claims;
using AiBot.Models;
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

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    // /Chat opens an empty new chat, /Chat/Index/5 opens conversation 5.
    public async Task<IActionResult> Index(int? id)
    {
        var userId = GetUserId();

        var conversations = await _db.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        var messages = new List<ChatMessage>();

        if (id != null)
        {
            // Only the owner can open a conversation.
            if (!conversations.Any(c => c.Id == id))
            {
                return NotFound();
            }

            messages = await _db.ChatMessages
                .Where(m => m.ConversationId == id)
                .OrderBy(m => m.Id)
                .ToListAsync();
        }

        return View(new ChatPageViewModel
        {
            Conversations = conversations,
            CurrentConversationId = id,
            Messages = messages
        });
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length > 1000)
        {
            return BadRequest("Please write a question (max 1000 characters).");
        }

        var userId = GetUserId();

        Conversation? conversation = null;
        var recent = new List<ChatMessage>();

        if (request.ConversationId != null)
        {
            conversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId && c.UserId == userId);

            if (conversation == null)
            {
                return NotFound();
            }

            // The last 6 messages give the bot some memory of this conversation.
            recent = await _db.ChatMessages
                .Where(m => m.ConversationId == conversation.Id)
                .OrderByDescending(m => m.Id)
                .Take(6)
                .ToListAsync();
            recent.Reverse();
        }

        var answer = await _chatAi.AskAsync(request.Question, recent);

        if (answer == null)
        {
            return Json(new
            {
                answer = "Sorry, I could not get an answer right now. Please try again.",
                conversationId = request.ConversationId
            });
        }

        if (conversation == null)
        {
            conversation = new Conversation
            {
                UserId = userId,
                Title = MakeTitle(request.Question),
                CreatedAtUtc = DateTime.UtcNow
            };
            _db.Conversations.Add(conversation);
        }

        conversation.Messages.Add(new ChatMessage
        {
            Role = "user",
            Text = request.Question,
            CreatedAtUtc = DateTime.UtcNow
        });
        conversation.Messages.Add(new ChatMessage
        {
            Role = "assistant",
            Text = answer,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return Json(new { answer, conversationId = conversation.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var userId = GetUserId();

        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (conversation != null)
        {
            _db.Conversations.Remove(conversation);
            await _db.SaveChangesAsync();
        }

        return Redirect("/Chat");
    }

    private static string MakeTitle(string question)
    {
        var title = question.Trim();
        return title.Length <= 40 ? title : title[..40] + "...";
    }
}

public record AskRequest(string Question, int? ConversationId);