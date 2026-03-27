using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using SemantiCore_API.Application.Interfaces;

namespace SemantiCore_API.Application.Services
{
    public class AzureOpenAIChatService : IAzureOpenAIChatService
    {
        private readonly ChatClient _chatClient;

        public AzureOpenAIChatService(IConfiguration configuration)
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"];

            var apiKey = configuration["AzureOpenAI:ApiKey"];

            var deploymentName = configuration["AzureOpenAI:ChatDeploymentName"];

            _chatClient = new ChatClient(
                credential: new ApiKeyCredential(apiKey),
                model: deploymentName,
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(endpoint)
                });
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            ChatCompletion completion =
                await _chatClient.CompleteChatAsync(
                    new[]
                    {
                        new SystemChatMessage(prompt)
                    });

            return completion.Content[0].Text;
        }
    }
}
