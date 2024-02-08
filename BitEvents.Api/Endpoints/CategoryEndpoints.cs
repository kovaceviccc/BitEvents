using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Domain.Enums;
using BitEvents.Api.Endpoints.Internal;
using BitEvents.Api.Mappers;
using BitEvents.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BitEvents.Api.Endpoints;

public sealed class CategoryEndpoints : IEndpoints
{
    private const string BaseRoute = "/categories";

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreateCategory)
            .RequireAuthorization(RolesEnum.Admin.ToString());
        app.MapGet(BaseRoute, GetAllCategories)
            .AllowAnonymous();
        app.MapGet(BaseRoute + "/{id}", GetCategoryById)
            .AllowAnonymous();
        app.MapPut(BaseRoute + "/{id}", UpdateCategory)
            .RequireAuthorization(RolesEnum.User.ToString());
        app.MapDelete(BaseRoute + "/{id}", DeleteCategory)
            .RequireAuthorization(RolesEnum.Admin.ToString());
    }

    internal static async Task<IResult> DeleteCategory(
        string id,
        ICategoryService categoryService)
    {
        var result = await categoryService.DeleteCategoryAsync(id);
        return result
            ? Results.Ok("Category successfully deleted")
            : Results.NotFound($"Category with the id of: {id} does not exist");
    }

    internal static async Task<IResult> UpdateCategory(
        string id,
        [FromBody] CategoryUpdateRequest categoryUpdateDto,
        ICategoryService categoryService,
        IValidator<CategoryUpdateRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(categoryUpdateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await categoryService.UpdateCategoryAsync(id, categoryUpdateDto);
        return category is null
            ? Results.NotFound("There is no category with specified id")
            : Results.Ok(CategoryMapper.CategoryToCategoryViewResponse(category));
    }

    internal static async Task<IResult> GetCategoryById(
        string id,
        ICategoryService categoryService)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        return category is null
            ? Results.NotFound($"Category with the id of: {id} does not exist")
            : Results.Ok(CategoryMapper.CategoryToCategoryViewResponse(category));
    }

    internal static async Task<IResult> GetAllCategories(
        [AsParameters] CategoryQueryFilter queryFilter,
        ICategoryService categoryService)
    {
        var categories = await categoryService.GetAllCategoriesAsync(queryFilter);
        var categoriesResponse = CategoryMapper.CategoryToCategoryViewResponseEnumerable(categories);
        return Results.Ok(categoriesResponse);
    }

    internal static async Task<IResult> CreateCategory(
        [FromBody] CategoryCreateRequest categoryCreateDto,
        ICategoryService categoryService,
        IValidator<CategoryCreateRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(categoryCreateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await categoryService.CreateCategoryAsync(categoryCreateDto);
        var categoryResponse = CategoryMapper.CategoryToCategoryViewResponse(category);
        return Results.Created($"{BaseRoute}/{category.Id}", categoryResponse);
    }
}