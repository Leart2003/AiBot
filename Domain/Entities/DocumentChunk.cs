using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DocumentChunk
    {
        public int Id { get; set; }
        public int CourseDocumentId { get; set; }
        public CourseDocument CourseDocument { get; set; } = null!;
        public int ChunkIndex { get; set; }
        public int? PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
