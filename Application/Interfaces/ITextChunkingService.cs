namespace SemantiCore_API.Application.Interfaces
{
    public interface ITextChunkingService
    {
        List<string> ChunkText(
            string text,
            int chunkSize = 800,
            int overlap = 100);
    }
}
