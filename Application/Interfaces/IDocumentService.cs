using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<IndexedDocument> UploadAsync(
            string categoryName,
            string indexName,
            IFormFile file);

        Task<DocumentResponseDto?> GetByIdAsync(int documentId);

        Task<DocumentViewModel?> GetDocumentForViewAsync(int documentId);
    }
}
