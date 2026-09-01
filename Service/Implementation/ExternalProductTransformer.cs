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

    public static List<string> ParseIngredientNames(string? ingredientsText)
    {
        if (string.IsNullOrWhiteSpace(ingredientsText))
            return new List<string>();

        return ingredientsText
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(CleanIngredientName)
            .Where(name => !string.IsNullOrWhiteSpace(name) && name.Length <= 100)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(30) 
            .ToList();
    }

    private static string CleanIngredientName(string raw)
    {
        var parenIndex = raw.IndexOf('(');
        var cleaned = parenIndex >= 0 ? raw[..parenIndex] : raw;
        return cleaned.Trim().TrimEnd('.', '*');
    }

    private static string ExtractFirstBrand(string? brands)
    {
        if (string.IsNullOrWhiteSpace(brands)) return "Unknown";
        return brands.Split(',')[0].Trim();
    }
}