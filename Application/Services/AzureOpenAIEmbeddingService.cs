using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;
using SemantiCore_API.Application.Interfaces;

namespace SemantiCore_API.Application.Services
{
    public class AzureOpenAIEmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public AzureOpenAIEmbeddingService(IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]);
            var apiKey = configuration["AzureOpenAI:ApiKey"];
            var deploymentName = configuration["AzureOpenAI:EmbeddingDeploymentName"];

            var client = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions
                {
                    Endpoint = endpoint
                });

            _embeddingClient = client.GetEmbeddingClient(deploymentName);
        }

        public async Task<float[]> GenerateEmbeddingAsync(string input)
        {
            ClientResult<OpenAIEmbedding> result =
                await _embeddingClient.GenerateEmbeddingAsync(input);

            return result.Value.ToFloats().ToArray();
        }
    }

}
