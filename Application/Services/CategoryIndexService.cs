using SemantiCore_API.Application.Interfaces;
using SemantiCore_API.Domain.Entities;
using SemantiCore_API.Infrastructure.Data;
using SemantiCore_API.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SemantiCore_API.Application.Services
{
    public class CategoryIndexService : ICategoryIndexService
    {
        private readonly SemantiCoreDbContext _context;

        public CategoryIndexService(SemantiCoreDbContext context)
        {
            _context = context;
        }

        // ✅ FIXED: Return IndexCategory
        public async Task<IndexCategory> CreateCategoryWithIndexesAsync( CreateCategoryIndexesDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var category = await _context.IndexCategories
                .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);

            if (category == null)
            {
                category = new IndexCategory
                {
                    CategoryName = dto.CategoryName
                };

                _context.IndexCategories.Add(category);
                await _context.SaveChangesAsync();
            }

            // Prevent duplicate index names in request
            if (dto.Indexes.GroupBy(x => x.IndexName).Any(g => g.Count() > 1))
                throw new InvalidOperationException("Duplicate index names");

            foreach (var input in dto.Indexes)
            {
                bool exists = await _context.CategoryIndexes.AnyAsync(x =>
                    x.IndexCategoryId == category.Id &&
                    x.IndexName == input.IndexName);

                if (exists)
                    throw new InvalidOperationException(
                        $"Index '{input.IndexName}' already exists");

                var index = new CategoryIndex
                {
                    IndexCategoryId = category.Id,
                    IndexType = input.IndexType,
                    IndexName = input.IndexName,
                    IndexValue = input.IndexValue
                };

                _context.CategoryIndexes.Add(index);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return category; // ✅ REQUIRED
        }

        // ✅ FIXED: Method name, param, return type
        public async Task<CategoryIndexesResponseDto?> GetByCategoryNameAsync(string categoryName)
        {
            var category = await _context.IndexCategories
                .Include(c => c.CategoryIndexes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryName == categoryName);

            if (category == null)
                return null;

            return new CategoryIndexesResponseDto
            {
                CategoryName = category.CategoryName,
                Indexes = category.CategoryIndexes.Select(i => new CategoryIndexResponseDto
                {
                    IndexType = i.IndexType,
                    IndexName = i.IndexName,
                    IndexValue = i.IndexValue
                }).ToList()
            };
        }

        public async Task<List<CategoryIndexesResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.IndexCategories
                .Include(c => c.CategoryIndexes)
                .AsNoTracking()
                .ToListAsync();

            return categories.Select(category => new CategoryIndexesResponseDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Indexes = category.CategoryIndexes.Select(i => new CategoryIndexResponseDto
                {
                    Id=i.Id,
                    IndexType = i.IndexType,
                    IndexName = i.IndexName,
                    IndexValue = i.IndexValue
                }).ToList()
            }).ToList();
        }

        public async Task AddIndexesByCategoryIdAsync(AddIndexesByCategoryDto dto)
        {
            var category = await _context.IndexCategories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

            if (category == null)
                throw new Exception("Category not found");

            // Prevent duplicate index names in request
            if (dto.Indexes.GroupBy(x => x.IndexName).Any(g => g.Count() > 1))
                throw new InvalidOperationException("Duplicate index names in request");

            foreach (var input in dto.Indexes)
            {
                bool exists = await _context.CategoryIndexes.AnyAsync(x =>
                    x.IndexCategoryId == dto.CategoryId &&
                    x.IndexName == input.IndexName);

                if (exists)
                    throw new InvalidOperationException(
                        $"Index '{input.IndexName}' already exists");

                var index = new CategoryIndex
                {
                    IndexCategoryId = dto.CategoryId,
                    IndexType = input.IndexType,
                    IndexName = input.IndexName,
                    IndexValue = input.IndexValue
                };

                _context.CategoryIndexes.Add(index);
            }

            await _context.SaveChangesAsync();
        }
    }
}
