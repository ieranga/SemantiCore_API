using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SemantiCore_API.Application.Services
{
    public class SemanticSearchService : ISemanticSearchService
    {
        private readonly SemantiCoreDbContext _context;
        private readonly IEmbeddingService _embeddingService;
        private readonly IAiResponseOrganizer _aiOrganizer;
        public SemanticSearchService(

            SemantiCoreDbContext context,
            IEmbeddingService embeddingService,
            IAiResponseOrganizer aiOrganizer)
        {
            _context = context;
            _embeddingService = embeddingService;
            _aiOrganizer = aiOrganizer;
        }

        public async Task<SemanticSearchResponseDto> SearchAsync(SemanticSearchRequestDto request)
        {
            // 1. Generate embedding
            var queryVector =
                await _embeddingService.GenerateEmbeddingAsync(request.Query);

            // 2. Load embeddings (your existing code)
            var embeddings = await _context.DocumentEmbeddings
                .Include(e => e.DocumentChunk)
                    .ThenInclude(c => c.IndexedDocument)
                        .ThenInclude(d => d.CategoryIndex)
                            .ThenInclude(ci => ci.IndexCategory)
                .AsNoTracking()
                .ToListAsync();

            // 3. Category filter (unchanged)
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                embeddings = embeddings
                    .Where(e =>
                        e.DocumentChunk
                         .IndexedDocument
                         .CategoryIndex
                         .IndexCategory
                         .CategoryName == request.CategoryName)
                    .ToList();
            }

            // 4. Similarity calculation (UNCHANGED)
            var rawResults = embeddings
                .Select(e =>
                {
                    var vector = JsonSerializer
                        .Deserialize<float[]>(e.Vector)!;

                    var score = CosineSimilarity(queryVector, vector);

                    return new SemanticSearchResultDto
                    {
                        DocumentChunkId = e.DocumentChunkId,
                        ChunkText = e.DocumentChunk.ChunkText,
                        Score = score
                    };
                })
                .OrderByDescending(r => r.Score)
                .Take(request.TopK)
                .ToList();

            // 🧠 5. AI ORGANIZATION (NEW)
            var organizedText = await _aiOrganizer.OrganizeAsync(
                request.Query,
                rawResults
            );

            // 6. Return organized response
            return new SemanticSearchResponseDto
            {
                Query = request.Query,
                OrganizedResult = organizedText,
                RawResults = rawResults // optional
            };
        }


        private static double CosineSimilarity(float[] v1, float[] v2)
        {
            double dot = 0;
            double mag1 = 0;
            double mag2 = 0;

            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                mag1 += v1[i] * v1[i];
                mag2 += v2[i] * v2[i];
            }

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }
}
