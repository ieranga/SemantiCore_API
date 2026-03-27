namespace SemantiCore_API.Shared.DTOs
{
    public class CreateCategoryIndexesDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<IndexInputDto> Indexes { get; set; } = new();
    }
    public class IndexInputDto
    {
        public int IndexType { get; set; }  // 1,2,3
        public string IndexName { get; set; } = string.Empty;
        public string IndexValue { get; set; } = string.Empty;
    }

    public class UploadIndexDocumentDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;

        public IFormFile File { get; set; } = null!;
    }

    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class CategoryIndexesResponseDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<CategoryIndexResponseDto> Indexes { get; set; } = new();
    }

    public class AddIndexesByCategoryDto
    {
        public int CategoryId { get; set; }

        public List<CategoryIndexResponseDto> Indexes { get; set; }
    }

    public class CategoryIndexResponseDto
    {
        public int Id { get; set; }
        public int IndexType { get; set; }
        public string IndexName { get; set; } = string.Empty;
        public string IndexValue { get; set; } = string.Empty;
    }

    public class SemanticSearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int TopK { get; set; } = 5;
    }
    public class SemanticSearchResultDto
    {
        public int DocumentChunkId { get; set; }
        public string ChunkText { get; set; } = string.Empty;
        public double Score { get; set; }
    }

    public class SemanticSearchResponseDto
    {
        public string Query { get; set; } = string.Empty;
        public string OrganizedResult { get; set; } = string.Empty;

        // Optional (for debugging / transparency)
        public List<SemanticSearchResultDto>? RawResults { get; set; }
    }

    

}
