using Microsoft.AspNetCore.Mvc;
using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController: ControllerBase
    {
        private readonly ISemanticSearchService _searchService;

        public SearchController(ISemanticSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("Semantic-Search")]
        public async Task<IActionResult> Search([FromBody]SemanticSearchRequestDto dto)
        {
            var results = await _searchService.SearchAsync(dto);
            return Ok(results);
        }
    }
}
