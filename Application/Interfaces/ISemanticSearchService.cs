using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Interfaces
{
    public interface ISemanticSearchService
    {
        Task<SemanticSearchResponseDto> SearchAsync(
            SemanticSearchRequestDto request);
    }

}
