namespace SemantiCore_API.Application.Interfaces
{
    public interface IDocumentTextExtractionService
    {
        Task<string> ExtractTextAsync(string filePath, string contentType);
    }
}
