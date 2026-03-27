using SemantiCore_API.Application.Interfaces;

namespace SemantiCore_API.Application.Services
{
    public class TextChunkingService : ITextChunkingService
    {
        public List<string> ChunkText(
            string text,
            int chunkSize = 800,
            int overlap = 100)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var chunks = new List<string>();

            int start = 0;

            while (start < text.Length)
            {
                int length = Math.Min(chunkSize, text.Length - start);
                string chunk = text.Substring(start, length);

                chunks.Add(chunk.Trim());

                start += chunkSize - overlap;
            }

            return chunks;
        }
    }
}
