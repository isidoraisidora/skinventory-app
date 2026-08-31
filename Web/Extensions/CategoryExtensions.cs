using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class CategoryExtensions
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse(category.Id, category.Name);
    }
}