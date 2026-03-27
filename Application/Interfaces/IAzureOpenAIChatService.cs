namespace SemantiCore_API.Application.Interfaces
{
    public interface IAzureOpenAIChatService
    {
        Task<string> GenerateAsync(string prompt);
    }
}
