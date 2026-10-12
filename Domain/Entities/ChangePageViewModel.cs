using Domain.Entities;

namespace AiBot.Models;

public class ChatPageViewModel
{
	public List<Conversation> Conversations { get; set; } = new();
	public int? CurrentConversationId { get; set; }
	public List<ChatMessage> Messages { get; set; } = new();
}