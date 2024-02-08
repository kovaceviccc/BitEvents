using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Contracts.Responses;
using BitEvents.Api.Models;
using Riok.Mapperly.Abstractions;

namespace BitEvents.Api.Mappers;

[Mapper]
public static partial class CategoryMapper
{
    public static partial Category CategoryCreateRequestToCategory(CategoryCreateRequest categoryCreateRequest);
    public static partial Category CategoryUpdateRequestToCategory(CategoryUpdateRequest categoryUpdateRequest);
    public static partial CategoryViewResponse CategoryToCategoryViewResponse(Category category);
    public static partial CategoryPartial CategoryToCategoryPartial(Category category);

    public static partial IEnumerable<CategoryViewResponse> CategoryToCategoryViewResponseEnumerable(
        IEnumerable<Category> categories);
}