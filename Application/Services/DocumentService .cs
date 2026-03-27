using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SemantiCore_API.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly SemantiCoreDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDocumentTextExtractionService _textExtractionService;
        private readonly ITextChunkingService _chunkingService;
        private readonly IEmbeddingService _embeddingService;

        public DocumentService(
            SemantiCoreDbContext context,
            IWebHostEnvironment env,
            IDocumentTextExtractionService textExtractionService,
            ITextChunkingService chunkingService,
            IEmbeddingService embeddingService)   // 👈 ADD THIS
        {
            _context = context;
            _env = env;
            _textExtractionService = textExtractionService;
            _chunkingService = chunkingService;
            _embeddingService = embeddingService; // 👈 ASSIGN IT
        }

        public async Task<IndexedDocument> UploadAsync(
            string categoryName,
            string indexName,
            IFormFile file)
        {
            // 1. Find category
            var category = await _context.IndexCategories
                .Include(c => c.CategoryIndexes)
                .FirstOrDefaultAsync(c => c.CategoryName == categoryName);

            if (category == null)
                throw new InvalidOperationException("Category not found");

            // 2. Find index
            var index = category.CategoryIndexes
                .FirstOrDefault(i => i.IndexName == indexName);

            if (index == null)
                throw new InvalidOperationException("Index not found");

            // 3. Ensure index is document type
            if (index.IndexType != 3)
                throw new InvalidOperationException("Index is not a document type");

            // 4. Save file
            var uploadRoot = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadRoot);

            var storedFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(uploadRoot, storedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Save document metadata
            var document = new IndexedDocument
            {
                CategoryIndexId = index.Id,
                OriginalFileName = file.FileName,
                StoredFilePath = fullPath,
                ContentType = file.ContentType ?? "",
                FileSize = file.Length
            };

            _context.IndexedDocuments.Add(document);
            await _context.SaveChangesAsync(); // Need ID

            // ===============================
            // 🔥 AI PIPELINE STARTS HERE
            // ===============================

            // 6. Extract text
            var extractedText = await _textExtractionService
                .ExtractTextAsync(fullPath, document.ContentType);

            // 7. Save extracted text (optional but recommended)
            _context.DocumentTexts.Add(new DocumentText
            {
                IndexedDocumentId = document.Id,
                ExtractedText = extractedText
            });

            // 8. Chunk text
            var chunks = _chunkingService.ChunkText(extractedText);

            // 9. Save chunks FIRST
            var chunkEntities = new List<DocumentChunk>();

            int order = 1;
            foreach (var chunk in chunks)
            {
                var chunkEntity = new DocumentChunk
                {
                    IndexedDocumentId = document.Id,
                    ChunkText = chunk,
                    ChunkOrder = order++
                };

                chunkEntities.Add(chunkEntity);
                _context.DocumentChunks.Add(chunkEntity);
            }

            // Save chunks to generate IDs
            await _context.SaveChangesAsync();

            // ===============================
            // 🔥 EMBEDDINGS GENERATION
            // ===============================

            // 10. Generate embeddings for each chunk
            foreach (var chunkEntity in chunkEntities)
            {
                var embeddingVector = await _embeddingService
                    .GenerateEmbeddingAsync(chunkEntity.ChunkText);

                _context.DocumentEmbeddings.Add(new DocumentEmbedding
                {
                    DocumentChunkId = chunkEntity.Id,
                    Vector = System.Text.Json.JsonSerializer.Serialize(embeddingVector)
                });
            }

            // Save embeddings
            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<DocumentResponseDto?> GetByIdAsync(int documentId)
        {
            var document = await _context.IndexedDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
                return null;

            return new DocumentResponseDto
            {
                Id = document.Id,
                FileName = document.OriginalFileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                UploadedAt = document.UploadedAt
            };
        }

        public async Task<DocumentViewModel?> GetDocumentForViewAsync(int documentId)
        {
            var document = await _context.IndexedDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
                return null;

            if (!System.IO.File.Exists(document.StoredFilePath))
                throw new FileNotFoundException("Stored file not found");

            return new DocumentViewModel
            {
                FileBytes = await System.IO.File.ReadAllBytesAsync(document.StoredFilePath),
                ContentType = document.ContentType
            };
        }

    }
}
