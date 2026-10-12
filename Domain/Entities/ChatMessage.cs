using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public string Role { get; set; } = string.Empty;   // "user" or "assistant"
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
