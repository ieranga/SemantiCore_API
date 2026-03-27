using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Services
{
    public class AiResponseOrganizer : IAiResponseOrganizer
    {
        private readonly IAzureOpenAIChatService _chatService;

        public AiResponseOrganizer(IAzureOpenAIChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<string> OrganizeAsync(
            string query,
            IEnumerable<SemanticSearchResultDto> results)
        {
            var combinedText = string.Join(
                "\n\n",
                results
                    .OrderByDescending(r => r.Score)
                    .Select(r => r.ChunkText)
            );

            var prompt = $@"
                    You are a document-structuring AI.

                    Instructions:
                    - Clean and organize the document content.
                    - Remove duplicated or repeated lines.
                    - Ignore irrelevant headers, footers, and navigation text.
                    - Identify the main section title.
                    - Group related content under clear headings.
                    - Use bullet points where appropriate.
                    - Do NOT add new information.
                    - Preserve original meaning.

                    User Query:
                    ""{query}""

                    Document Content:
                    <<<
                    {combinedText}
                    >>>

                    Return a well-structured, readable document section.
                    ";

            return await _chatService.GenerateAsync(prompt);
        }
    }
}
