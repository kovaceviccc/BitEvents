using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Models;
using BitEvents.Api.Exceptions;
using BitEvents.Api.Mappers;
using BitEvents.Api.Repositories.Interfaces;
using BitEvents.Api.Services.Interfaces;

namespace BitEvents.Api.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllCategoriesAsync(CategoryQueryFilter categoryQueryFilter)
    {
        return await _categoryRepository.GetAllCategoriesAsync(categoryQueryFilter);
    }

    public async Task<Category?> GetCategoryByIdAsync(string id)
    {
        return await _categoryRepository.GetCategoryByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(CategoryCreateRequest categoryCreateRequest)
    {
        var existingCategory = await _categoryRepository.GetCategoryWithNameAsync(categoryCreateRequest.Name);
        if (existingCategory is not null)
        {
            throw new InvalidInputException("Category with given name already exists");
        }

        var category = CategoryMapper.CategoryCreateRequestToCategory(categoryCreateRequest);
        var success = await _categoryRepository.CreateCategoryAsync(category);
        if (!success)
        {
            throw new DatabaseException("Failed to create category");
        }

        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(string id, CategoryUpdateRequest categoryUpdateRequest)
    {
        var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
        if (existingCategory is null)
        {
            return null;
        }

        existingCategory.Name = categoryUpdateRequest.Name;
        existingCategory.UpdatedAtUtc = DateTime.UtcNow;
        var success = await _categoryRepository.UpdateCategoryAsync(existingCategory);
        if (!success)
        {
            throw new DatabaseException("Failed to update the category");
        }

        return existingCategory;
    }

    public async Task<bool> DeleteCategoryAsync(string id)
    {
        var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
        if (existingCategory is null)
        {
            return false;
        }

        existingCategory.IsDeleted = true;
        existingCategory.DeletedAtUtc = DateTime.UtcNow;
        return await _categoryRepository.UpdateCategoryAsync(existingCategory);
    }
}