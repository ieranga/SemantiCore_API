using System.ComponentModel.DataAnnotations;

namespace SemantiCore_API.Domain.Entities
{
    public class IndexedDocument
    {
        [Key]
        public int Id { get; set; }

        public int CategoryIndexId { get; set; }

        public CategoryIndex CategoryIndex { get; set; } = null!;

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFilePath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // 🔴 ADD THIS
        public ICollection<DocumentChunk> DocumentChunks { get; set; }
            = new List<DocumentChunk>();
    }


    public class DocumentViewModel
    {
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
    }

    public class DocumentText
    {
        [Key]
        public int Id { get; set; }

        public int IndexedDocumentId { get; set; }

        [Required]
        public string ExtractedText { get; set; } = string.Empty;

        public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
    }

    public class DocumentChunk
    {
        [Key]
        public int Id { get; set; }

        public int IndexedDocumentId { get; set; }

        // 🔴 ADD THIS
        public IndexedDocument IndexedDocument { get; set; } = null!;

        public string ChunkText { get; set; } = string.Empty;

        public int ChunkOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DocumentEmbedding> DocumentEmbeddings { get; set; }
            = new List<DocumentEmbedding>();
    }


    public class DocumentEmbedding
    {
        [Key]
        public int Id { get; set; }

        public int DocumentChunkId { get; set; }

        // 🔴 ADD THIS (navigation)
        public DocumentChunk DocumentChunk { get; set; } = null!;

        // Vector stored as JSON string for SQL Server compatibility
        public string Vector { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
