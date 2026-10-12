using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = "New chat";
        public DateTime CreatedAtUtc { get; set; }

        public List<ChatMessage> Messages { get; set; } = new();
    }
}
