using System.Net.Http.Json;
using Domain.Dtos;
using Domain.Models;
using Service.Interface;

namespace Service.Implementation;

public class ExternalProductApi : IExternalProductApi
{
    private readonly HttpClient _httpClient;

    public ExternalProductApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> SearchProductsAsync(string categoryTag, int page, int pageSize)
    {
        var categoryParam = string.IsNullOrWhiteSpace(categoryTag)
            ? ""
            : $"categories_tags={categoryTag}&";
        
        
        var url = $"api/v2/search?categories_tags={categoryTag}&page={page}&page_size={pageSize}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Cannot read from API.");

        var raw = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"RAW (first 800 chars): {raw.Substring(0, Math.Min(800, raw.Length))}");

        var result = await response.Content.ReadFromJsonAsync<ExternalProductResponseDto>();

        if (result?.Products == null)
            throw new InvalidOperationException("Failed to parse API response.");

        if (result.Products.Count == 0)
            return new List<Product>();

        foreach (var dto in result.Products.Take(5))
        {
            Console.WriteLine($"Sample — Code: '{dto.Code}' | Name: '{dto.ProductName}' | Brand: '{dto.Brands}'");
        }

        return result.Products
            .Where(dto => !string.IsNullOrWhiteSpace(dto.Code) && !string.IsNullOrWhiteSpace(dto.ProductName))
            .Select(ExternalProductTransformer.ToProduct)
            .ToList();
    }
}