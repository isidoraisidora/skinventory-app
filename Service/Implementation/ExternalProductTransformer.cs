using Domain.Dtos;
using Domain.Models;

namespace Service.Implementation;

public static class ExternalProductTransformer
{
    public static Product ToProduct(ExternalProductDto dto)
    {
        return new Product
        {
            Name = string.IsNullOrWhiteSpace(dto.ProductName) ? "Unknown product" : dto.ProductName,
            Brand = ExtractFirstBrand(dto.Brands),
            Description = dto.IngredientsText,
            Barcode = dto.Code
        };
    }

    private static string ExtractFirstBrand(string? brands)
    {
        if (string.IsNullOrWhiteSpace(brands)) return "Unknown";
        return brands.Split(',')[0].Trim();
    }
}