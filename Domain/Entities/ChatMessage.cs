using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;   
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
