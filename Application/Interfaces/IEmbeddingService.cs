namespace SemantiCore_API.Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string input);
    }
}
