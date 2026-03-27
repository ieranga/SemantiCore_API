using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Interfaces
{
    public interface IAiResponseOrganizer
    {
        Task<string> OrganizeAsync(
            string query,
            IEnumerable<SemanticSearchResultDto> results
        );
    }
}
