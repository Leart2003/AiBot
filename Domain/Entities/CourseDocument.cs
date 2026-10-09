using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CourseDocument
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty; // safe server-side name
        public string ContentType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public CourseDocumentType Type { get; set; }
        public DateTime UploadedAtUtc { get; set; }

        public List<DocumentChunk> Chunks { get; set; } = new();
    }
}
