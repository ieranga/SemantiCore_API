using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SemantiCore_API.Controllers
{
    [ApiController]
    [Route("api/Categries")]
    public class CategoryIndexController : ControllerBase
    {
        private readonly ICategoryIndexService _service;

        public CategoryIndexController(ICategoryIndexService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategry(CreateCategoryIndexesDto dto)
        {
            await _service.CreateCategoryWithIndexesAsync(dto);
            return Ok("Category and indexes created successfully");
        }

        [HttpPost("addIndexesByCategoryId")]
        public async Task<IActionResult> AddIndexesByCategoryId(AddIndexesByCategoryDto dto)
        {
            await _service.AddIndexesByCategoryIdAsync(dto);
            return Ok("Indexes added successfully");
        }

        [HttpGet("{categoryName}")]
        public async Task<IActionResult> GetByCategoryId(string categoryName)
        {
            var result = await _service.GetByCategoryNameAsync(categoryName);

            if (result == null)
                return NotFound($"Category '{categoryName}' not found");

            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _service.GetAllCategoriesAsync();
            return Ok(categories);
        }

    }
}
