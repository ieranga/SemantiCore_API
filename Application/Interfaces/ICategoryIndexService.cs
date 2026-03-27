using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Shared.DTOs;

namespace SemantiCore_API.Application.Interfaces
{
    public interface ICategoryIndexService
    {
        Task<IndexCategory> CreateCategoryWithIndexesAsync(CreateCategoryIndexesDto dto);

        Task<CategoryIndexesResponseDto?> GetByCategoryNameAsync(string categoryName);

        Task AddIndexesByCategoryIdAsync(AddIndexesByCategoryDto dto);

        Task<List<CategoryIndexesResponseDto>> GetAllCategoriesAsync();
    }
} 
